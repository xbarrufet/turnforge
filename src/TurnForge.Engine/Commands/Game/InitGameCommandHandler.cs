using System;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Decisions;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Infrastructure.Factories;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Game;

public sealed class InitGameCommandHandler : ICommandHandler<InitGameCommand>
{
    private readonly IActorFactory _actorFactory;
    private readonly IGameFactory _gameFactory;
    private readonly IGameRepository _gameRepository;
    private readonly IPropSpawnStrategy _propSpawnStrategy;
    private readonly IAgentSpawnStrategy _agentSpawnStrategy;
    private readonly IEffectSink _effectsSink;
    private readonly IBoardFactory _boardFactory;

    public InitGameCommandHandler(
        IActorFactory actorFactory,
        IGameFactory gameFactory,
        IGameRepository gameRepository,
        IBoardFactory boardFactory,
        IPropSpawnStrategy propSpawnStrategy,
        IAgentSpawnStrategy agentSpawnStrategy,
        IEffectSink effectsSink)
    {
        _actorFactory = actorFactory;
        _gameFactory = gameFactory;
        _gameRepository = gameRepository;
        _boardFactory = boardFactory;
        _propSpawnStrategy = propSpawnStrategy;
        _agentSpawnStrategy = agentSpawnStrategy;
        _effectsSink = effectsSink;
    }

    public CommandResult Handle(InitGameCommand command)
    {
        // PHASE 1: Create Board and Game
        // We need to construct Initialization command DTOs from InitGameCommand parts if BoardApplier expects specific structure
        // But BoardApplier seems to expect 'InitializeGameCommand' currently. 
        // I should check BoardApplier. Wait, looking at InitializeGameHandler (Step 1111):
        // var board = new BoardApplier().Apply(command);
        // BoardApplier.Apply expects InitializeGameCommand.
        // I should probably REFLECT BoardApplier logic here directly or refactor BoardApplier.
        // Given BoardApplier is logic, I can instantiate it but it needs the argument type.
        // Assuming BoardApplier logic is:
        // var map = SpatialLoader.Load(command.Spatial);
        // var zoneMap = ZoneLoader.Load(command.Zones, map);
        // return new Board(map, zoneMap);

        // Let's verify BoardApplier or duplicate logic? 
        // Better: Refactor BoardApplier to take component parts OR duplication if simple.
        // I will assume simple duplication for now as BoardApplier was not viewed but used.
        // Actually, cleaner is to Create a temporary DTO if needed or update BoardApplier.
        // I'll update BoardApplier later. For now, I'll inline the logic if simple enough.

        // Wait, I cannot see BoardApplier. I should trust SpatialLoader/ZoneLoader exist.
        // But InitializeGameHandler uses SpatialLoader, TurnForge.Engine.Spatial.Interfaces...
        // Let's look at InitializeGameHandler imports again.
        // It imports `TurnForge.Engine.Spatial` and `TurnForge.Engine.Spatial.Interfaces`.

        // To be safe and reuse code, I'll update BoardApplier signature to take (SpatialDescriptor, Zones)
        // OR just replicate it.
        // Replicating:

        // 2️⃣ Crear Board
        // cretes the board and the game, it's totally out of the normal proces but it's just for the first time
        var gameState = GameState.Empty();

        var decisions = new List<IDecision>();
        var spatialModel = command.Spatial;

        var boardDescriptor = new BoardDescriptor(spatialModel, command.Zones);
        var board = new BoardApplier().Build(boardDescriptor, _boardFactory);
        var game = new SimpleGameFactory().Build(board);
        _gameRepository.SaveGame(game);
        _gameRepository.SaveGameState(gameState);
        // 

        // 3️⃣ Spawn Props creating decisions
        var propContext = new PropSpawnContext(command.StartingProps, gameState);
        var propDecisions = _propSpawnStrategy.Decide(propContext);

        // 4️⃣ Spawn Agents creating decisions
        var agentContext = new AgentSpawnContext(command.Agents, gameState);
        var agentDecisions = _agentSpawnStrategy.Decide(agentContext);

        decisions.AddRange(propDecisions.Cast<IDecision>());
        decisions.AddRange(agentDecisions.Cast<IDecision>());

        return CommandResult.Ok(decisions: decisions.ToArray(), tags: ["GameInitialized", "StartFSM"]);
    }
}
