using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

public sealed class LoadGameHandler
    : ICommandHandler<LoadGameCommand>
{
    private readonly IActorFactory _actorFactory;
    private readonly IGameRepository _gameRepository;
    private readonly IPropSpawnStrategy _propSpawnStrategy;
    private readonly IEffectSink _effectsSink;

    public LoadGameHandler(
        IActorFactory actorFactory,
        IGameRepository gameRepository,
        IPropSpawnStrategy propSpawnStrategy,
        IEffectSink effectsSink)
    {
        _actorFactory = actorFactory;
        _gameRepository = gameRepository;
        _propSpawnStrategy = propSpawnStrategy;
        _effectsSink = effectsSink;
    }

    public CommandResult Handle(LoadGameCommand command)
    {
        // 1️⃣ Construir SpatialModel
        var spatialModel = BuildSpatialModel(command.Spatial);

        // 2️⃣ Crear Board
        var board = new GameBoard(spatialModel);

        // 3️⃣ Crear Game
        var game = new Game(new GameId(), board);
        var gameState = game.GetGameState();

        // 4️⃣ Crear actores (DECISIÓN + EJECUCIÓN)
        //build context for spawn strategy
        var context = new PropSpawnContext(command.Props,new ReadOnlyGameState(gameState)); 
        var decisions = _propSpawnStrategy.Decide(context);
        //crear SpwanApplier
        var spawner = new SpawnApplier(gameState, _actorFactory, _effectsSink);
        foreach (var decision in decisions)
        {
            spawner.Spawn(decision);
        }
        // 5️⃣ Guardar
        _gameRepository.SaveGame(game);
        return CommandResult.Ok();
    }

    private ISpatialModel BuildSpatialModel(
        SpatialDescriptor spatial)
    {
        return spatial switch
        {
            DiscreteSpatialDescriptor d =>
                new ConnectedGraphSpatialModel(
                    new MutableTileGraph(
                        d.Connections.Select(c => (c.From, c.To))
                    )
                ),

            ContinuousSpatialDescriptior =>
                throw new NotImplementedException(),

            _ => throw new NotSupportedException()
        };
    }
}