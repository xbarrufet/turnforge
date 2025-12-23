using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using BarelyAlive.Rules.Core.Domain.ValueObjects;
using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Handlers;

public class AgentSpawnedProjector : IEffectProjector
{
    public bool CanHandle(IGameEffect effect) => effect is AgentSpawnedResult;

    public void Project(IGameEffect effect, ICollection<EntityBuildUpdate> created, ICollection<EntityStateUpdate> updated, ICollection<DomainEvent> events)
    {
        if (effect is not AgentSpawnedResult agent) return;

        // Assuming EntityBuildUpdate constructor: new EntityBuildUpdate(string Id, string Type, string DefinitionId, string TileId, Dictionary<string, object> InitialState)
        // Previous error said "There is no argument given that corresponds to the required parameter 'InitialState'".
        // I will add the empty dictionary as InitialState.
        
        created.Add(new EntityBuildUpdate(
            agent.AgentId.Value.ToString(),
            "Agent", // Type
            agent.DefinitionId, // Definition/Variation
            agent.Position.TileId.ToString(),
            new List<string>(), // Behaviors
            new Dictionary<string, object>() // InitialState
        ));
    }
}
