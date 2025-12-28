
using System;
using System.Collections.Generic;
using System.Linq;
using global::TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.ACK;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Orchestrator.Interfaces;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Core.State;
using TurnForge.Engine.Core.Logging;

namespace TurnForge.Engine.Core;

public sealed class GameEngineRuntime : IGameEngine
{
    private readonly CommandBus _commandBus;
    private readonly IGameRepository _repository;
    private readonly IOrchestrator _orchestrator;
    private readonly IGameLogger _logger;
    private FsmController? _fsmController;
    private readonly IWorkflowOrchestrator _workflowOrchestrator;
    private IWorkflow? _activeWorkflow;
    private WorkflowContext? _activeWorkflowContext;

    private readonly bool _useCommandTransation = true; // Default

    private readonly IBoardFactory _boardFactory;

    public GameEngineRuntime(CommandBus commandBus, IGameRepository repository, IOrchestrator orchestrator, IWorkflowOrchestrator workflowOrchestrator, IGameLogger logger, IBoardFactory boardFactory)
    {
        _commandBus = commandBus;
        _repository = repository;
        _orchestrator = orchestrator;
        _workflowOrchestrator = workflowOrchestrator;
        _logger = logger;
        _boardFactory = boardFactory;

        _orchestrator.SetLogger(_logger);
    }

    public void SetFsmController(FsmController controller)
    {
        _fsmController = controller;
        _fsmController.SetOrchestrator(_orchestrator);
        _fsmController.SetLogger(_logger);
    }

    // SUMMARY:
    // Main method of the Engine. Orchestrate the command execution and FSM transition
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
                        throw new InvalidOperationException($"Cannot execute command {command.GetType().Name} while workflow {_activeWorkflow.Id} is suspended. Waiting for input.");
                    }
                }
                else
                {
                    // Should generic commands be allowed while workflow is running? 
                    // Usually workflow runs synchronously, so we shouldn't be here unless async/threads.
                    // Assuming single-thread, this is safe. 
                }
            }

            //1 - validates we are not in ACK waiting state and command is not an ACK command
            if (_fsmController != null && IsAckValidCommand(_fsmController.WaittingForACK, command))
            {
                transaction.Result = CommandResult.ACKResult;
                return transaction;
            }

            //2- check if we are in a valid state to execute the command
            if (_fsmController != null)
            {
                ValidateIfValidState(_fsmController.CurrentNode, command);
            }

            //3- execute the command
            var result = _commandBus.Send(command);

            //4- react with FSM (if active and command was successful)
            if (result.Success && _fsmController != null)
            {
                var state = _repository.LoadGameState();
                var events = new List<IGameEvent>();
                // Sync Orchestrator
                _orchestrator.SetState(state);
                // Enqueue new decisions (persists them to scheduler)
                if (result.Decisions.Any())
                {
                    _orchestrator.Enqueue(result.Decisions);
                    state = _orchestrator.CurrentState; // Update state with new scheduler
                }

                // Execute Immediate Decisions (OnCommandExecutionEnd)
                var immediateEvents = _orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd");
                events.AddRange(immediateEvents);
                state = _orchestrator.CurrentState; // Update state after application
                
                // Standard reaction (and auto-navigation)
                var stepResult = _fsmController.HandleCommand(command, state, result);
                events.AddRange(stepResult.GameEvents); // Assuming StepResult updated to GameEvents
                
                _repository.SaveGameState(stepResult.State);

                if (stepResult.IsGameOver)
                {
                    _logger.LogInfo("Game Over detected. Stopping flow.");
                    transaction.IsGameOver = true;
                    transaction.Result = result;
                    transaction.Events = events.ToArray();
                    return transaction;
                }
                
                // 5- Auto-Launch Command (Recursion)
                if (stepResult.CommandToLaunch != null)
                {
                    _logger.LogInfo($"Auto-Launching Command: {stepResult.CommandToLaunch.GetType().Name}", LogContext.ForCommand(stepResult.CommandToLaunch.GetType().Name));
                    var subTransaction = ExecuteCommand(stepResult.CommandToLaunch);
                    
                    // Merge events from valid execution
                    var mergedEvents = new List<IGameEvent>(events);
                    if (subTransaction.Events != null) mergedEvents.AddRange(subTransaction.Events);
                    
                    subTransaction.Events = mergedEvents.ToArray();
                    return subTransaction;
                }

                // 6- Launch Workflow (FSM Integration)
                if (stepResult.WorkflowToLaunch != null)
                {
                    _logger.LogInfo($"FSM Launching Workflow: {stepResult.WorkflowToLaunch.Id}");
                    return StartWorkflow(stepResult.WorkflowToLaunch, stepResult.WorkflowContext, transaction, events);
                }

                // activate ACK state in FSM 
                _fsmController.WaittingForACK = true;
                transaction.Result = CommandResult.ACKResult;
                transaction.Events = [.. events];
                return transaction;
            }
            else if (result.Success && _fsmController == null)
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
            //5- handle exception. send a failure response
            _logger.LogError($"Error executing command {command.GetType().Name}", ex, LogContext.ForCommand(command.GetType().Name));
            transaction.Result = CommandResult.Fail(ex.Message);
            return transaction;
        }

    }

    private void ValidateIfValidState(FsmNode currentState, ICommand command)
    {
        if (!currentState.IsCommandAllowed(command.GetType()))
        {
            throw new Exception($"Command {command.GetType().Name} not allowed in state {currentState.Id}");
        }
    }

    public bool IsAckValidCommand(bool weAreaInWaitinfForAck, ICommand command)
    {
        if (weAreaInWaitinfForAck)
        {
            if (command.CommandType != typeof(ACKCommand))
            {
                throw new Exception("Command is not an ACK command");
            }
            else
            {
                _fsmController.WaittingForACK = false;
                return true;
            }
        }
        return command is ACKCommand;
    }


    private CommandTransaction StartWorkflow(IWorkflow workflow, WorkflowContext? context, CommandTransaction transaction, List<IGameEvent> priorEvents)
    {
        _activeWorkflow = workflow;
        // Ensure context exists
        _activeWorkflowContext = context ?? new GenericWorkflowContext(); 
        
        // Initialize working state from current game state
        _activeWorkflowContext.InitializeState(_orchestrator.CurrentState);

        var executionResult = _workflowOrchestrator.Execute(workflow, _activeWorkflowContext);

        if (executionResult.Status == WorkflowStatus.Completed)
        {
            return CompleteWorkflow(transaction, priorEvents);
        }
        else if (executionResult.Status == WorkflowStatus.Suspended)
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
        if (_activeWorkflow == null || _activeWorkflowContext == null) throw new InvalidOperationException("No active workflow");

        var executionResult = _workflowOrchestrator.Resume(
            _activeWorkflow, 
            _activeWorkflowContext, 
            command.Input);

        if (executionResult.Status == WorkflowStatus.Completed)
        {
            return CompleteWorkflow(transaction, new List<IGameEvent>());
        }
        else
        {
            transaction.Result = CommandResult.Ok(Array.Empty<IDecision>());
            transaction.Events = Array.Empty<IGameEvent>(); // Or accumulated events?
            return transaction;
        }
    }

    private CommandTransaction CompleteWorkflow(CommandTransaction transaction, List<IGameEvent> priorEvents)
    {
        // Atomic Commit
        var events = new List<IGameEvent>(priorEvents);
        
        if (_activeWorkflowContext != null)
        {
            foreach(var decision in _activeWorkflowContext.Decisions)
            {
               events.AddRange(_orchestrator.Apply(decision));
            }
            
            // Also collect events from the workflow execution?
            events.AddRange(_activeWorkflowContext.PendingEvents.OfType<IGameEvent>());
        }
        
        // Save State
        _repository.SaveGameState(_orchestrator.CurrentState);
        
        // Clear Workflow
        _activeWorkflow = null;
        _activeWorkflowContext = null;
        
        // Trigger FSM Update (Recursive?)
        if (_fsmController != null)
        {
             // We need to fetch the latest state
             var currentState = _orchestrator.CurrentState;
             var fsmResult = _fsmController.MoveForwardRequest(currentState);
             events.AddRange(fsmResult.GameEvents);
             
             // Handle FSM Result (Transitions, Game Over, New Commands...)
             // Simplified: Just return events for now. 
             // Ideally we should handle recursion if FSM triggers another command/workflow.
             // But for Phase 7 MVP, we'll stop here.
        }

        transaction.Result = CommandResult.Ok(Array.Empty<IDecision>(), "WORKFLOW_COMPLETED");
        transaction.Events = events.ToArray();
        return transaction;
    }

    private class GenericWorkflowContext : WorkflowContext { }

    // Temporary placeholder for projector until injected
    private class ReturnSameStateProjector : IStateProjector
    {
        public GameState Project(GameState baseState, IEnumerable<IDecision> decisions)
        {
             return baseState;
        }
    }

    public void Subscribe(Action<IGameEvent> handler)
    {
        // Removed EffectSink subscription
    }
}