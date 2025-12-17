using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
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
        var game = _repo.GetCurrent();
        if (game is null)
            return CommandResult.Fail("No current game to start");
        var gameState = game.GetGameState();
        if (gameState is null)
            return CommandResult.Fail("Game state not available");

        var context = new UnitSpawnContext(command.PlayerUnits, new ReadOnlyGameState(gameState));
        var decisions = _unitSpawnStrategy.Decide(context);

        var spawner = new SpawnApplier(gameState, _actorFactory, _effectsSink);
        foreach (var decision in decisions)
        {
            spawner.Spawn(decision);
        }
        _repo.SaveGame(game);
        return CommandResult.Ok(tags: ["GameStarted"]);
    }
}
