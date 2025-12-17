using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Definitions;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

public sealed class LoadGameHandler
    : ICommandHandler<LoadGameCommand>
{
    private readonly IActorFactory _actorFactory;
    private readonly IGameRepository _gameRepository;

    public LoadGameHandler(
        IActorFactory actorFactory,
        IGameRepository gameRepository)
    {
        _actorFactory = actorFactory;
        _gameRepository = gameRepository;
    }

    public void Handle(LoadGameCommand command)
    {
        // 1️⃣ Construir SpatialModel
        var spatialModel = BuildSpatialModel(command.Spatial);

        // 2️⃣ Crear Board
        var board = new GameBoard(spatialModel);

        // 3️⃣ Crear Game
        var game = new Game(new GameId(), board);

        // 4️⃣ Crear actores
        foreach (var def in command.Actors)
        {
            var actor = _actorFactory.CreateActor(def);
            game.AddActor(actor);
        }

        // 5️⃣ Guardar
        _gameRepository.SaveGame(game);
    }

    private ISpatialModel BuildSpatialModel(
        SpatialDefinition spatial)
    {
        return spatial switch
        {
            DiscreteSpatialDefinition d =>
                new ConnectedGraphSpatialModel(
                    new MutableTileGraph(
                        d.Connections.Select(c => (c.From, c.To))
                    )
                ),

            ContinuousSpatialDefinition =>
                throw new NotImplementedException(),

            _ => throw new NotSupportedException()
        };
    }
}