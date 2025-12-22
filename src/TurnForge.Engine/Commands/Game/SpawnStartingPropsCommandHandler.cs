using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.ValueObjects;
namespace TurnForge.Engine.Commands.Game;

public sealed class SpawnStartingPropsCommandHandler : ICommandHandler<SpawnStartingPropsCommand>
{
    private readonly IGameRepository _gameRepository;
    private readonly IPropSpawnStrategy _propSpawnStrategy;

    public SpawnStartingPropsCommandHandler(
        IGameRepository gameRepository,
        IPropSpawnStrategy propSpawnStrategy)
    {
        _gameRepository = gameRepository;
        _propSpawnStrategy = propSpawnStrategy;
    }

    public CommandResult Handle(SpawnStartingPropsCommand command)
    {
        // We load the current state from the repository.
        // It is expected that InitializeBoard has already been called and the state contains the Board.
        var gameState = _gameRepository.LoadGameState();
        
        var decisions = new List<IDecision>();

        // Spawn Props creating decisions
        var propContext = new PropSpawnContext(command.StartingProps, gameState);
        var propDecisions = _propSpawnStrategy.Decide(propContext);

        decisions.AddRange(propDecisions.Cast<IDecision>());

        return CommandResult.Ok(decisions: decisions.ToArray(), tags: ["StartingPropsSpawned"]);
    }
}
