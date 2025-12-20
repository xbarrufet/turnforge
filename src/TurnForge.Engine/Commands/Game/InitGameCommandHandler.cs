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
        var gameState = GameState.Empty();
        var decisions = new List<IDecision>();

        // 1️⃣ Create Board Decision
        var boardDescriptor = new BoardDescriptor(command.Spatial, command.Zones);
        decisions.Add(new BoardDecision(boardDescriptor));

        // 2️⃣ Spawn Props creating decisions
        var propContext = new PropSpawnContext(command.StartingProps, gameState);
        var propDecisions = _propSpawnStrategy.Decide(propContext);

        // 3️⃣ Spawn Agents creating decisions
        var agentContext = new AgentSpawnContext(command.Agents, gameState);
        var agentDecisions = _agentSpawnStrategy.Decide(agentContext);

        decisions.AddRange(propDecisions.Cast<IDecision>());
        decisions.AddRange(agentDecisions.Cast<IDecision>());

        return CommandResult.Ok(decisions: decisions.ToArray(), tags: ["GameInitialized", "StartFSM"]);
    }
}
