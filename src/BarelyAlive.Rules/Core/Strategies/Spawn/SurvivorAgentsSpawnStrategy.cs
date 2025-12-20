using BarelyAlive.Rules.Core.Behaviours.ActorBehaviours;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Strategies.Spawn;

public class SurvivorAgentsSpawnStrategy : IAgentSpawnStrategy
{

    private Position DetermineSpawnLocation(AgentSpawnContext ctx)
    {
        // Look for PartySpawnActor points
        var spawnPoint = ctx.GameState.GetProps()
            .FirstOrDefault(p => p.Definition.TypeId.Value == "PartySpawnActor");

        if (spawnPoint == null) return Position.Empty;

        return spawnPoint.GetComponent<PositionComponent>().CurrentPosition;

    }

    public IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext ctx)
    {
        var decision = new List<AgentSpawnDecision>();
        foreach (var agentDescriptor in ctx.AgentsToSpawn)
        {
            // For simplicity, spawn all units at the same position
            var spawnPosition = DetermineSpawnLocation(ctx);
            decision.Add(new AgentSpawnDecision(agentDescriptor.TypeId, spawnPosition,
                agentDescriptor.ExtraBehaviours ?? Array.Empty<IActorBehaviour>()));
        }
        return decision;
    }
}
