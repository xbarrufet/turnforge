
using TurnForge.Engine.Entities;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Strategies.Spawn;

public sealed class AgentNpcSpawnContext(GameState gameState) : ISpawnContext
{
    public GameState GameState { get; } = gameState;
}
