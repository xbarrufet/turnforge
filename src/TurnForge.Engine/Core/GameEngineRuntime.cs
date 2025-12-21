
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
using TurnForge.Engine.Orchestrator;
using TurnForge.Engine.Orchestrator.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class GameEngineRuntime(CommandBus commandBus, IEffectSink effectSink, IGameRepository repository, IOrchestrator orchestrator) : IGameEngine
{
    private readonly CommandBus _commandBus = commandBus;
    private readonly IGameRepository _repository = repository;
    private readonly IOrchestrator _orchestrator = orchestrator;
    private FsmController? _fsmController;

    public void SetFsmController(FsmController controller)
    {
        _fsmController = controller;
        _fsmController.SetOrchestrator(_orchestrator);
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
                // Standard reaction
                var stepResult = _fsmController.HandleCommand(command, state, result);
                effects.AddRange(stepResult.Effects);
                // we need to check if there is a transition
                _repository.SaveGameState(stepResult.State);
                if (stepResult.TransitionRequested)
                {
                    var transitionResult = _fsmController.MoveForwardRequest(stepResult.State, effectSink);
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
            return transaction;
        }
        catch (Exception ex)
        {
            //5- handle exception. send a failure response
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
        effectSink.Subscribe(handler);
    }
}