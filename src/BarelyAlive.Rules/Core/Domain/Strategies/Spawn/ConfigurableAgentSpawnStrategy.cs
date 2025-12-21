using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Spawn;

public class ConfigurableAgentSpawnStrategy : IAgentSpawnStrategy
{
    public IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext ctx)
    {
        // Simple default: Allow all spawning for now
        var decisions = new List<AgentSpawnDecision>();
        foreach (var request in ctx.AgentsToSpawn)
        {
            decisions.Add(new AgentSpawnDecision(request.TypeId, request.Position ?? TurnForge.Engine.ValueObjects.Position.Empty, request.ExtraBehaviours));
        }
        return decisions;
    }
}
