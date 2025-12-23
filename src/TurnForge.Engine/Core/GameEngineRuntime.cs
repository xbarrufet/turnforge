
using System;
using System.Collections.Generic;
using System.Linq;
using global::TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.ACK;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Core.Orchestrator.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class GameEngineRuntime : IGameEngine
{
    private readonly CommandBus _commandBus;
    private readonly IGameRepository _repository;
    private readonly IOrchestrator _orchestrator;
    private readonly IGameLogger _logger;
    private FsmController? _fsmController;

    private readonly bool _useCommandTransation = true; // Default

    private readonly IBoardFactory _boardFactory;

    public GameEngineRuntime(CommandBus commandBus, IGameRepository repository, IOrchestrator orchestrator, IGameLogger logger, IBoardFactory boardFactory)
    {
        _commandBus = commandBus;
        _repository = repository;
        _orchestrator = orchestrator;
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
                var effects = new List<IGameEffect>();
                // Sync Orchestrator
                _orchestrator.SetState(state);
                // Enqueue new decisions (persists them to scheduler)
                if (result.Decisions.Any())
                {
                    _orchestrator.Enqueue(result.Decisions);
                    state = _orchestrator.CurrentState; // Update state with new scheduler
                }
                
                // Standard reaction (and auto-navigation)
                var stepResult = _fsmController.HandleCommand(command, state, result);
                effects.AddRange(stepResult.Effects);
                
                _repository.SaveGameState(stepResult.State);
                
                // 5- Auto-Launch Command (Recursion)
                if (stepResult.CommandToLaunch != null)
                {
                    _logger.Log($"[Engine] Auto-Launching Command: {stepResult.CommandToLaunch.GetType().Name}");
                    // Execute the auto-launched command immediately
                    // Note: We return the result of the chain. 
                    // Effects from previous steps are lost? No, we should merge them?
                    // Typically, if a command triggers another, the UI receives the FINAL sequence.
                    // But effectively, this is a distinct transaction.
                    // For simplicity, we return the transaction of the launched command, 
                    // BUT UI might miss effects of the trigger?
                    // Ideally, we chain transactions or effects.
                    // IMPORTANT: TurnForge returns single Transaction.
                    // If we recurse, we get a NEW transaction.
                    // We must merge effects?
                    
                    var subTransaction = ExecuteCommand(stepResult.CommandToLaunch);
                    
                    // Merge effects from valid execution
                    var mergedEffects = new List<IGameEffect>(effects);
                    if (subTransaction.Effects != null) mergedEffects.AddRange(subTransaction.Effects);
                    
                    subTransaction.Effects = mergedEffects.ToArray();
                    return subTransaction;
                }

                // activate ACK state in FSM 
                _fsmController.WaittingForACK = true;
                transaction.Result = CommandResult.ACKResult;
                transaction.Effects = [.. effects];
                return transaction;
            }
            transaction.Result = result;
            _logger.Log($"Command {command.GetType().Name} executed. Success: {result.Success}");
            return transaction;
        }
        catch (Exception ex)
        {
            //5- handle exception. send a failure response
            _logger.LogError($"Error executing command {command.GetType().Name}", ex);
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
            if (command.CommandType != typeof(CommandAck))
            {
                throw new Exception("Command is not an ACK command");
            }
            else
            {
                _fsmController.WaittingForACK = false;
                return true;
            }
        }
        return command is CommandAck;
    }


    public void Subscribe(Action<IGameEffect> handler)
    {
        // Removed EffectSink subscription
    }
}