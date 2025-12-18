using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure.Appliers;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Commands.GameStart;

public sealed class GameStartCommandHandler
    : ICommandHandler<GameStartCommand>
{
    private readonly IGameRepository _repo;
    private readonly IUnitSpawnStrategy _unitSpawnStrategy;
    private readonly IEffectSink _effectsSink;
    private readonly IActorFactory _actorFactory;

    public GameStartCommandHandler(
        IGameRepository repo,
        IUnitSpawnStrategy unitSpawnStrategy,
        IActorFactory actorFactory,
        IEffectSink effectsSink)
    {
        _repo = repo;
        _actorFactory = actorFactory;
        _effectsSink = effectsSink;
        _unitSpawnStrategy = unitSpawnStrategy;
    }

    public CommandResult Handle(GameStartCommand command)
    {
        var gameState = _repo.Load();
        var context = new UnitSpawnContext(command.PlayerUnits, gameState);
        var decisions = _unitSpawnStrategy.Decide(context);

        var spawner = new SpawnApplier(_actorFactory, _effectsSink);
        var newState = spawner.Apply(decisions, gameState);
        _repo.SaveGameState(newState);
        return CommandResult.Ok(tags: ["GameStarted"]);
    }
}
