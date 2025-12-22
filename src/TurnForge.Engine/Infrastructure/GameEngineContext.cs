using TurnForge.Engine.Registration;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure;

public readonly struct GameEngineContext(
    IGameRepository gameRepository,
    IPropSpawnStrategy propSpawnStrategy,
    IAgentSpawnStrategy agentSpawnStrategy,
    TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null)
{
    public IGameRepository GameRepository { get; init; } = gameRepository;

    public IPropSpawnStrategy PropSpawnStrategy { get; init; } = propSpawnStrategy;
    public IAgentSpawnStrategy AgentSpawnStrategy { get; init; } = agentSpawnStrategy;
    public TurnForge.Engine.Core.Interfaces.IGameLogger Logger { get; init; } = logger ?? new ConsoleLogger();
}
