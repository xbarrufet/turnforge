
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
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
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
    private FsmGraph? _fsmGraph;
    private readonly IWorkflowOrchestrator _workflowOrchestrator;
    private IWorkflow? _activeWorkflow;
    private WorkflowContext? _activeWorkflowContext;

    private bool _waitingForAck;
    private readonly IOrchestrator _orchestrator;
    private readonly IBoardFactory _boardFactory;

    public GameEngineRuntime(CommandBus commandBus, IGameRepository repository, IOrchestrator orchestrator, IWorkflowOrchestrator workflowOrchestrator, IGameLogger logger, IBoardFactory boardFactory)
    {
        _commandBus = commandBus;
        _repository = repository;
        _orchestrator = orchestrator;
        _workflowOrchestrator = workflowOrchestrator;
        _logger = logger;
        _boardFactory = boardFactory;
    }

    public void SetFsmGraph(FsmGraph graph)
    {
        _fsmGraph = graph;
    }

    /// <summary>
    /// Main method of the Engine. Orchestrates command execution and FSM transitions.
    /// </summary>
    public CommandTransaction ExecuteCommand(ICommand command)
    {
        var transaction = new CommandTransaction(command);
        try
        {
            // 0 - Intercept Workflow Input (if active)
            if (_activeWorkflow != null && _activeWorkflowContext != null)
            {
                if (_activeWorkflowContext.Status == WorkflowStatus.Suspended)
                {
                    if (command is WorkflowInputCommand inputCommand)
                    {
                        return ResumeWorkflow(inputCommand, transaction);
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

    private CommandTransaction StartWorkflow(IWorkflow workflow, WorkflowContext? context, CommandTransaction transaction, List<IGameEvent> priorEvents)
    {
        _activeWorkflow = workflow;
        _activeWorkflowContext = context ?? new GenericWorkflowContext(); 
        _activeWorkflowContext.InitializeState(_orchestrator.CurrentState);

        if (!Guid.TryParse(workflow.Id.Value, out var guid))
        {
            guid = Guid.NewGuid(); 
        }

        _workflowOrchestrator.StartWorkflow(workflow, _activeWorkflowContext);

        if (_activeWorkflowContext.Status == WorkflowStatus.Completed)
        {
            return CompleteWorkflow(transaction, priorEvents);
        }
        else if (_activeWorkflowContext.Status == WorkflowStatus.Suspended)
        {
            transaction.Result = CommandResult.Ok(Array.Empty<IDecision>(), "WORKFLOW_STARTED", "SUSPENDED");
            transaction.Events = priorEvents.ToArray();
            return transaction;
        }
        else 
        {
            throw new Exception("Workflow Cancelled or Failed on Start");
        }
    }

    private CommandTransaction ResumeWorkflow(WorkflowInputCommand command, CommandTransaction transaction)
    {
        if (_activeWorkflow == null || _activeWorkflowContext == null) 
            throw new InvalidOperationException("No active workflow");

        if (!Guid.TryParse(_activeWorkflow.Id.Value, out var guid))
        {
            guid = Guid.NewGuid();
        }

        _workflowOrchestrator.SubmitInput(guid, command.Input);

        if (_activeWorkflowContext.Status == WorkflowStatus.Completed)
        {
            return CompleteWorkflow(transaction, new List<IGameEvent>());
        }
        else
        {
            transaction.Result = CommandResult.Ok(Array.Empty<IDecision>());
            transaction.Events = Array.Empty<IGameEvent>();
            return transaction;
        }
    }

    private CommandTransaction CompleteWorkflow(CommandTransaction transaction, List<IGameEvent> priorEvents)
    {
        var events = new List<IGameEvent>(priorEvents);
        
        if (_activeWorkflowContext != null)
        {
            foreach(var decision in _activeWorkflowContext.Decisions)
            {
               events.AddRange(_orchestrator.Apply(decision));
            }
            
            events.AddRange(_activeWorkflowContext.PendingEvents.OfType<IGameEvent>());
        }
        
        _repository.SaveGameState(_orchestrator.CurrentState);
        
        _activeWorkflow = null;
        _activeWorkflowContext = null;
        
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

    private class GenericWorkflowContext : WorkflowContext 
    {
        public override object? GetResult() => null;
    }

    public void Subscribe(Action<IGameEvent> handler)
    {
        // Removed EffectSink subscription
    }
}