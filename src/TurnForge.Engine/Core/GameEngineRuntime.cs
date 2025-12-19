using global::TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.GameStart;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;



public sealed class GameEngineRuntime(CommandBus commandBus, IEffectSink effectSink, IGameRepository repository) : IGameEngine
{
    private readonly CommandBus _commandBus = commandBus;
    private readonly IGameRepository _repository = repository;
    private FsmController? _fsmController;

    public void SetFsmController(FsmController controller)
    {
        _fsmController = controller;
        // Subscribe to FSM transitions
        _fsmController.OnTransitionExecuted += ApplyFsmTransition;
    }

    private void ApplyFsmTransition(IEnumerable<Infrastructure.Appliers.Interfaces.IFsmApplier> appliers)
    {
        var state = _repository.LoadGameState();
        foreach (var applier in appliers)
        {
            state = applier.Apply(state, effectSink);
        }
        _repository.SaveGameState(state);
    }

    public CommandResult InitializeGame(InitializeGameCommand command)
    {
        return _commandBus.Send(command);
    }

    public CommandResult StartGame(GameStartCommand command)
    {
        return _commandBus.Send(command);
    }

    public CommandResult Send(ICommand command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        // 1. Execute Core Logic
        var result = command switch
        {
            InitializeGameCommand initCommand => InitializeGame(initCommand),
            GameStartCommand startCommand => StartGame(startCommand),
            _ => _commandBus.Send(command)
        };

        // 2. React with FSM (if active and command was successful)
        if (result.Success && _fsmController != null)
        {
            var state = _repository.LoadGameState();
            var appliers = _fsmController.HandleCommand(command, state, result);

            if (appliers.Any())
            {
                foreach (var applier in appliers)
                {
                    state = applier.Apply(state, effectSink);
                }
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