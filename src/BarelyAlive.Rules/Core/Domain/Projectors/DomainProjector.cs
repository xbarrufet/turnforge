using System.Collections.Generic;

using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Handlers;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using TurnForge.Engine.Core.Orchestrator;

namespace BarelyAlive.Rules.Core.Domain.Projectors;

public class DomainProjector
{
    private readonly IEnumerable<IEffectProjector> _projectors;

    public DomainProjector()
    {
        // In a real DI scenario, these would be injected.
        _projectors = new IEffectProjector[]
        {
            new AgentSpawnedProjector(),
            new PropSpawnedProjector()
        };
    }

    public GameUpdatePayload CreatePayload(CommandTransaction transaction)
    {
        var created = new List<EntityBuildUpdate>();
        var updated = new List<EntityStateUpdate>();
        var events = new List<DomainEvent>();

        foreach (var effect in transaction.Effects)
        {
            foreach (var projector in _projectors)
            {
                if (projector.CanHandle(effect))
                {
                    projector.Project(effect, created, updated, events);
                }
            }
        }

        return new GameUpdatePayload(created, updated, events);
    }
}
