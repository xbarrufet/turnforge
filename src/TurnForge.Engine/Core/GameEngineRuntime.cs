
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
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
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
                ValidateIfValidState(_fsmController.CurrentState, command);
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
                // Standard reaction, here is where we process the command effects in the FSM
                var stepResult = _fsmController.HandleCommand(command, state, result);
                effects.AddRange(stepResult.Effects);
                // we need to check if there is a transition
                _repository.SaveGameState(stepResult.State);
                if (stepResult.TransitionRequested)
                {
                    var transitionResult = _fsmController.MoveForwardRequest(stepResult.State);
                    effects.AddRange(transitionResult.Effects);
                    _repository.SaveGameState(transitionResult.State);
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