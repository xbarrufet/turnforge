
using System;
using System.Collections.Generic;
using System.Linq;
using global::TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Repositories.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class GameEngineRuntime(CommandBus commandBus, IEffectSink effectSink, IGameRepository repository) : IGameEngine
{
    private readonly CommandBus _commandBus = commandBus;
    private readonly IGameRepository _repository = repository;
    private FsmController? _fsmController;

    public void SetFsmController(FsmController controller)
    {
        _fsmController = controller;
    }

    public CommandResult Send(ICommand command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        // 1. Execute Core Logic
        var result = _commandBus.Send(command);

        // 2. React with FSM (if active and command was successful)
        if (result.Success && _fsmController != null)
        {
            var state = _repository.LoadGameState();
            if (result.Tags.Contains("StartFSM"))
            {
                // Force initial FSM movement
                state = _fsmController.MoveForwardRequest(state, effectSink);
                _repository.SaveGameState(state);
                return result;
            }
            // Standard reaction
            var stepResult = _fsmController.HandleCommand(command, state, result, effectSink);
            // we need to check if there is a transition
            _repository.SaveGameState(stepResult.State);
            if (stepResult.TransitionRequested)
            {
                state = _fsmController.MoveForwardRequest(stepResult.State, effectSink);
                _repository.SaveGameState(state);
            }
        }
        return result;
    }

    public void Subscribe(Action<IGameEffect> handler)
    {
        effectSink.Subscribe(handler);
    }
}