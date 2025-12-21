using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using BarelyAlive.Rules.Core.Domain.ValueObjects;
using TurnForge.Engine.Entities.Appliers.Results;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Handlers;

public class AgentSpawnedProjector : IEffectProjector
{
    public bool CanHandle(IGameEffect effect) => effect is AgentSpawnedResult;

    public void Project(IGameEffect effect, ICollection<EntityBuildUpdate> created, ICollection<EntityStateUpdate> updated, ICollection<DomainEvent> events)
    {
        if (effect is not AgentSpawnedResult agent) return;

        created.Add(new EntityBuildUpdate(
            agent.AgentId.Value.ToString(),
            "Agent",
            agent.AgentType.Value,
            new Vector(agent.Position.X, agent.Position.Y),
            new Dictionary<string, object>()
        ));
    }
}
