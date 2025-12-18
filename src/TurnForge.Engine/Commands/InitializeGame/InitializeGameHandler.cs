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
using TurnForge.Engine.Infrastructure.Appliers;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

public sealed class InitializeGameHandler
    : ICommandHandler<InitializeGameCommand>
{
    private readonly IActorFactory _actorFactory;
    private readonly IGameRepository _gameRepository;
    private readonly IGameFactory _gameFactory;
    private readonly IPropSpawnStrategy _propSpawnStrategy;
    private readonly IEffectSink _effectsSink;

    public InitializeGameHandler(
        IActorFactory actorFactory,
        IGameFactory gameFactory,
        IGameRepository gameRepository,
        IPropSpawnStrategy propSpawnStrategy,
        IEffectSink effectsSink)
    {
        _actorFactory = actorFactory;
        _gameFactory = gameFactory;
        _gameRepository = gameRepository;
        _propSpawnStrategy = propSpawnStrategy;
        _effectsSink = effectsSink;
    }

    public CommandResult Handle(InitializeGameCommand command)
    {
        
        // 2️⃣ Crear Board
        var board = new BoardApplier().Apply(command);
        // 3️⃣ Crear Game
        var game = _gameFactory.Build(board);
        var gameState = GameState.Empty(); 
        // 4️⃣ Crear actores (DECISIÓN + EJECUCIÓN)
        //build context for spawn strategy
        var context = new PropSpawnContext(command.StartingProps,gameState); 
        var decisions = _propSpawnStrategy.Decide(context);
        //crear SpwanApplier y aplicar
        var spawner = new SpawnApplier( _actorFactory, _effectsSink);
        spawner.Apply(decisions, gameState);
        
        // 5️⃣ Guardar
        _gameRepository.SaveGame(game);
        _gameRepository.SaveGameState(gameState);
        return CommandResult.Ok();
    }

   
}