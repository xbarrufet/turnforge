using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Strategies;

public class TestAgentSpawnStrategy : IAgentSpawnStrategy
{
    public IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext context)
    {
        List<AgentSpawnDecision> decisions = new();
        foreach (var descriptor in context.AgentsToSpawn)
        {
            decisions.Add(new AgentSpawnDecision(descriptor.TypeId, new Position(Vector.Zero), new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()));
        }
        return decisions;
    }
}