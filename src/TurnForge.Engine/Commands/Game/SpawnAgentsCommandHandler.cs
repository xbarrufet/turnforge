
using System.Linq;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Commands.Game;

public sealed class SpawnAgentsCommandHandler(
    IAgentSpawnStrategy agentSpawnStrategy,
    IGameRepository gameRepository
) : ICommandHandler<SpawnAgentsCommand>
{
    private readonly IAgentSpawnStrategy _agentSpawnStrategy = agentSpawnStrategy;
    private readonly IGameRepository _gameRepository = gameRepository;

    public CommandResult Handle(SpawnAgentsCommand command)
    {
        // Load current state (Board should exist from InitGame)
        var gameState = _gameRepository.LoadGameState();

        // Create spawn decisions for player agents
        var context = new AgentSpawnContext(command.PlayerAgents, gameState);
        var decisions = _agentSpawnStrategy.Decide(context);

        // Return decisions and trigger transition
        return CommandResult.Ok(
            decisions: decisions.Cast<IDecision>().ToArray(),
            tags: ["AgentsSpawned"]
        );
    }
}
