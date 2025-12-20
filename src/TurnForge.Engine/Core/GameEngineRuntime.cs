
using System;
using System.Collections.Generic;
using System.Linq;
using global::TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
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

    public CommandResult Send(ICommand command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        // 1. Execute Core Logic
        var result = _commandBus.Send(command);

        // 2. React with FSM (if active and command was successful)
        if (result.Success && _fsmController != null)
        {
            var state = _repository.LoadGameState();

            // Sync Orchestrator
            _orchestrator.SetState(state);

            // Enqueue new decisions (persists them to scheduler)
            if (result.Decisions.Any())
            {
                _orchestrator.Enqueue(result.Decisions);
                state = _orchestrator.CurrentState; // Update state with new scheduler
            }

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