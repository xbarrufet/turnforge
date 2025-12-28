using TurnForge.Engine.Core.Metrics;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure;

public readonly struct GameEngineContext(
    IGameRepository gameRepository,
    ISpawnStrategy<PropDescriptor>? propSpawnStrategy,
    ISpawnStrategy<AgentDescriptor>? agentSpawnStrategy,
    TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null,
    IEngineMetrics? metrics = null)
{
    public IGameRepository GameRepository { get; init; } = gameRepository;

    public ISpawnStrategy<PropDescriptor>? PropSpawnStrategy { get; init; } = propSpawnStrategy;
    public ISpawnStrategy<AgentDescriptor>? AgentSpawnStrategy { get; init; } = agentSpawnStrategy;
    public TurnForge.Engine.Core.Interfaces.IGameLogger Logger { get; init; } = logger ?? new ConsoleLogger();
    public IEngineMetrics Metrics { get; init; } = metrics ?? NullMetrics.Instance;
}
