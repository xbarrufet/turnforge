
using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.ACK;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Logging;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Decisions;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class GameEngineRuntime : IGameEngine
{
    private readonly CommandBus _commandBus;
    private readonly IGameRepository _repository;
    private readonly IGameLogger _logger;
    private readonly IActionRegistry _workflowRegistry;
    private FsmGraph? _fsmGraph;
    private readonly IActionOrchestrator _workflowOrchestrator;
    private IAction? _activeAction;
    private ActionContext? _activeActionContext;
    private GameStatus _gameStatus = GameStatus.WaitingForStart;

    private bool _waitingForAck;
    private readonly IOrchestrator _orchestrator;
    private readonly IBoardFactory _boardFactory;

    public GameEngineRuntime(
        CommandBus commandBus, 
        IGameRepository repository, 
        IOrchestrator orchestrator, 
        IActionOrchestrator workflowOrchestrator, 
        IActionRegistry workflowRegistry,
        IGameLogger logger, 
        IBoardFactory boardFactory)
    {
        _commandBus = commandBus;
        _repository = repository;
        _orchestrator = orchestrator;
        _workflowOrchestrator = workflowOrchestrator;
        _workflowRegistry = workflowRegistry;
        _logger = logger;
        _boardFactory = boardFactory;
    }

    public void SetFsmGraph(FsmGraph graph)
    {
        _fsmGraph = graph;
    }

    /// <summary>
    /// Execute a workflow by its registered ID.
    /// This is the primary API for external systems (UI) to trigger game actions.
    /// </summary>
    public ActionTransaction ExecuteAction(ActionId workflowId, Dictionary<string, object>? parameters = null)
    {
        // 0. Check Game Status
        if (_gameStatus == GameStatus.GameOver)
        {
            return ActionTransaction.Fail(workflowId, "Game is over. Call ResetGame() to start a new game.");
        }
        
        try
        {
            // Update status if starting
            if (_gameStatus == GameStatus.WaitingForStart)
            {
                _gameStatus = GameStatus.InProgress;
            }

            // 1. Get workflow from registry
            var workflow = _workflowRegistry.GetAction(workflowId);
            if (workflow == null)
            {
                return ActionTransaction.Fail(workflowId, $"Action '{workflowId.Value}' not registered.");
            }
            
            // 2. Create context with current state
            var state = _repository.LoadGameState();
            var context = new GenericActionContext();
            context.InitializeState(state);
            
            // 3. Inject parameters into context
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    if (kvp.Key == "BatchInputs" && kvp.Value is IEnumerable<IActionInput> inputs)
                    {
                        foreach (var input in inputs)
                        {
                            context.EnqueueInput(input);
                        }
                    }
                    else
                    {
                        context.Set(kvp.Key, kvp.Value);
                    }
                }
            }
            
            // Inject Internal Services
            context.Set("System.BoardFactory", _boardFactory);
            
            // 4. Execute workflow
            _workflowOrchestrator.StartAction(workflow, context);
            
            // 5. Handle result based on status
            if (context.Status == ActionStatus.Completed)
            {
                // Get events from overlay
                var events = context.Overlay.GetEvents()
                    .Select(op => new ActionOperationEvent(op))
                    .Cast<IGameEvent>()
                    .ToList();
                
                // Commit overlay to state
                var newState = context.Overlay.Commit();
                _repository.SaveGameState(newState);
                
                // Process FSM if active
                if (_fsmGraph != null)
                {
                    var fsmResult = _fsmGraph.ProcessFlow(newState);
                    events.AddRange(fsmResult.Events);
                    
                    if (fsmResult.IsGameOver)
                    {
                        _gameStatus = GameStatus.GameOver;
                        return new ActionTransaction(workflowId) 
                        { 
                            Status = ActionStatus.Completed, 
                            Events = events, 
                            IsGameOver = true 
                        };
                    }
                }
                
                return ActionTransaction.Success(workflowId, events);
            }
            else if (context.Status == ActionStatus.Suspended)
            {
                _activeAction = workflow;
                _activeActionContext = context;
                return ActionTransaction.Suspended(workflowId);
            }
            else
            {
                var msg = context.ErrorMessage ?? "Action failed or was cancelled.";
                return ActionTransaction.Fail(workflowId, msg);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error executing workflow {workflowId.Value}", ex);
            return ActionTransaction.Fail(workflowId, ex.Message);
        }
    }

    /// <summary>
    /// Get current game status.
    /// </summary>
    public GameStatus GetStatus() => _gameStatus;

    /// <summary>
    /// Reset the game. Clears state, resets FSM to root, and returns to WaitingForStart.
    /// </summary>
    public void ResetGame()
    {
        _logger.LogInfo("Resetting game engine...");
        
        // 1. Clear Active Actions
        _activeAction = null;
        _activeActionContext = null;
        _waitingForAck = false;
        
        // 2. Reset Status
        _gameStatus = GameStatus.WaitingForStart;
        
        // 3. Reset FSM
        if (_fsmGraph != null)
        {
            _fsmGraph.Reset();
        }
        
        // 4. Reset State (Empty)
        var emptyState = Entities.GameState.Empty();
        _repository.SaveGameState(emptyState);
        _orchestrator.SetState(emptyState);
        
        _logger.LogInfo("Game engine reset complete.");
    }

    /// <summary>
    /// Main method of the Engine. Orchestrates command execution and FSM transitions.
    /// </summary>
    public CommandTransaction ExecuteCommand(ICommand command)
    {
        var transaction = new CommandTransaction(command);
        try
        {
            // 0 - Intercept Action Input (if active)
            if (_activeAction != null && _activeActionContext != null)
            {
                if (_activeActionContext.Status == ActionStatus.Suspended)
                {
                    if (command is ActionInputCommand inputCommand)
                    {
                        return ResumeAction(inputCommand, transaction);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot execute command {command.GetType().Name} while workflow is suspended.");
                    }
                }
            }

            // 1 - Validate ACK state
            if (_waitingForAck && IsAckValidCommand(command))
            {
                transaction.Result = CommandResult.ACKResult;
                return transaction;
            }

            // 2 - Check if command is allowed in current FSM state
            if (_fsmGraph != null)
            {
                ValidateCommandAllowed(command);
            }

            // 3 - Execute the command
            var result = _commandBus.Send(command);

            // 4 - React with FSM (if active and command was successful)
            if (result.Success && _fsmGraph != null)
            {
                var state = _repository.LoadGameState();
                var events = new List<IGameEvent>();
                
                // Sync Orchestrator
                _orchestrator.SetState(state);
                
                // Enqueue new decisions
                if (result.Decisions.Any())
                {
                    _orchestrator.Enqueue(result.Decisions);
                    state = _orchestrator.CurrentState;
                }

                // Execute Immediate Decisions (OnCommandExecutionEnd)
                var immediateEvents = _orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd");
                events.AddRange(immediateEvents);
                state = _orchestrator.CurrentState;
                
                // Process FSM flow (auto-transitions, resolvers)
                var fsmResult = _fsmGraph.ProcessFlow(state);
                events.AddRange(fsmResult.Events);
                state = fsmResult.State;
                
                _repository.SaveGameState(state);

                if (fsmResult.IsGameOver)
                {
                    _logger.LogInfo("Game Over detected. Stopping flow.");
                    transaction.IsGameOver = true;
                    transaction.Result = result;
                    transaction.Events = events.ToArray();
                    return transaction;
                }
                
                // Activate ACK state
                _waitingForAck = true;
                transaction.Result = CommandResult.ACKResult;
                transaction.Events = events.ToArray();
                return transaction;
            }
            else if (result.Success && _fsmGraph == null)
            {
                // NO-FSM Fallback: Apply decisions and save state
                var state = _repository.LoadGameState();
                _orchestrator.SetState(state);
                
                var events = new List<IGameEvent>();
                foreach (var d in result.Decisions)
                {
                    events.AddRange(_orchestrator.Apply(d));
                }
                
                _repository.SaveGameState(_orchestrator.CurrentState);
                
                transaction.Result = result;
                transaction.Events = events.ToArray();
                return transaction;
            }

            transaction.Result = result;
            _logger.LogInfo($"Command {command.GetType().Name} executed. Success: {result.Success}", LogContext.ForCommand(command.GetType().Name));
            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error executing command {command.GetType().Name}", ex, LogContext.ForCommand(command.GetType().Name));
            transaction.Result = CommandResult.Fail(ex.Message);
            return transaction;
        }
    }

    private void ValidateCommandAllowed(ICommand command)
    {
        if (_fsmGraph != null && !_fsmGraph.IsCommandAllowed(command.GetType()))
        {
            throw new Exception($"Command {command.GetType().Name} not allowed in current state {_fsmGraph.CurrentNode.Name}");
        }
    }

    private bool IsAckValidCommand(ICommand command)
    {
        if (_waitingForAck)
        {
            if (command.CommandType != Commands.ValueObjects.CommandType.ACK)
            {
                throw new Exception("Waiting for ACK command");
            }
            else
            {
                _waitingForAck = false;
                return true;
            }
        }
        return command is ACKCommand;
    }

    private CommandTransaction StartAction(IAction workflow, ActionContext? context, CommandTransaction transaction, List<IGameEvent> priorEvents)
    {
        _activeAction = workflow;
        _activeActionContext = context ?? new GenericActionContext(); 
        _activeActionContext.InitializeState(_orchestrator.CurrentState);

        if (!Guid.TryParse(workflow.Id.Value, out var guid))
        {
            guid = Guid.NewGuid(); 
        }

        _workflowOrchestrator.StartAction(workflow, _activeActionContext);

        if (_activeActionContext.Status == ActionStatus.Completed)
        {
            return CompleteAction(transaction, priorEvents);
        }
        else if (_activeActionContext.Status == ActionStatus.Suspended)
        {
            transaction.Result = CommandResult.Ok(Array.Empty<IDecision>(), "WORKFLOW_STARTED", "SUSPENDED");
            transaction.Events = priorEvents.ToArray();
            return transaction;
        }
        else 
        {
            throw new Exception("Action Cancelled or Failed on Start");
        }
    }

    private CommandTransaction ResumeAction(ActionInputCommand command, CommandTransaction transaction)
    {
        if (_activeAction == null || _activeActionContext == null) 
            throw new InvalidOperationException("No active workflow");

        if (!Guid.TryParse(_activeAction.Id.Value, out var guid))
        {
            guid = Guid.NewGuid();
        }

        _workflowOrchestrator.SubmitInput(guid, command.Input);

        if (_activeActionContext.Status == ActionStatus.Completed)
        {
            return CompleteAction(transaction, new List<IGameEvent>());
        }
        else
        {
            transaction.Result = CommandResult.Ok(Array.Empty<IDecision>());
            transaction.Events = Array.Empty<IGameEvent>();
            return transaction;
        }
    }

    private CommandTransaction CompleteAction(CommandTransaction transaction, List<IGameEvent> priorEvents)
    {
        var events = new List<IGameEvent>(priorEvents);
        
        if (_activeActionContext != null)
        {
            foreach(var decision in _activeActionContext.Decisions)
            {
               events.AddRange(_orchestrator.Apply(decision));
            }
            
            events.AddRange(_activeActionContext.PendingEvents.OfType<IGameEvent>());
        }
        
        _repository.SaveGameState(_orchestrator.CurrentState);
        
        _activeAction = null;
        _activeActionContext = null;
        
        // Trigger FSM Update
        if (_fsmGraph != null)
        {
            var currentState = _orchestrator.CurrentState;
            var fsmResult = _fsmGraph.ProcessFlow(currentState);
            events.AddRange(fsmResult.Events);
        }

        transaction.Result = CommandResult.Ok(Array.Empty<IDecision>(), "WORKFLOW_COMPLETED");
        transaction.Events = events.ToArray();
        return transaction;
    }

    private class GenericActionContext : ActionContext 
    {
        public override object? GetResult() => null;
    }

    public void Subscribe(Action<IGameEvent> handler)
    {
        // Removed EffectSink subscription
    }
}