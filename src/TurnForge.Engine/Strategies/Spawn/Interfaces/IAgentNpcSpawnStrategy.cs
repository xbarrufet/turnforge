
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface IAgentNpcSpawnStrategy
{
    IReadOnlyList<AgentSpawnDecision> Decide(AgentNpcSpawnContext context);
}
