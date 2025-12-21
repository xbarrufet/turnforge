using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Commands.Game;

public class StartGameCommandHandler : ICommandHandler<StartGameCommand>
{
    private readonly IAgentSpawnStrategy _agentSpawnStrategy;
    private readonly IGameRepository _gameRepository;

    public StartGameCommandHandler(IAgentSpawnStrategy agentSpawnStrategy, IGameRepository gameRepository)
    {
        _agentSpawnStrategy = agentSpawnStrategy;
        _gameRepository = gameRepository;
    }

    public CommandResult Handle(StartGameCommand command)
    {
        var gameState = _gameRepository.LoadGameState();
        // Assuming LoadGameState returns a valid state if game is initialized. 
        // If it throws or returns empty on uninitialized, we might need a check, 
        // but InitGame must have run.

        var decisions = new List<IDecision>();

        var agentContext = new AgentSpawnContext(command.Agents, gameState);
        var agentDecisions = _agentSpawnStrategy.Decide(agentContext);

        decisions.AddRange(agentDecisions.Cast<IDecision>());

        return CommandResult.Ok(decisions: decisions.ToArray(), tags: ["GameStarted"]);
    }
}
