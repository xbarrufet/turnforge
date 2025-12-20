using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Descriptors;

using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Strategies.Spawn;

public sealed class AgentSpawnContext(IReadOnlyList<AgentDescriptor> agentsToSpawn, GameState gameState)
    : ISpawnContext
{
    public GameState GameState { get; } = gameState;
    public IReadOnlyList<AgentDescriptor> AgentsToSpawn { get; } = agentsToSpawn;
}
