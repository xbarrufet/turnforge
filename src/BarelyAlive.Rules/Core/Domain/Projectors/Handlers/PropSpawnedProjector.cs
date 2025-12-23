using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using BarelyAlive.Rules.Core.Domain.ValueObjects;
using TurnForge.Engine.Appliers.Effects;
using TurnForge.Engine.Appliers.Entity.Effects;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Handlers;

public class PropSpawnedProjector : IEffectProjector
{
    public bool CanHandle(IGameEffect effect) => effect is PropSpawnedEffect;

    public void Project(IGameEffect effect, ICollection<EntityBuildUpdate> created, ICollection<EntityStateUpdate> updated, ICollection<DomainEvent> events)
    {
        if (effect is not PropSpawnedEffect prop) return;
        
        created.Add(new EntityBuildUpdate(
            prop.PropId.ToString(),
            "Prop",
            prop.DefinitionId,
            prop.Position.TileId.ToString(),
            new List<string>(), // Behaviors
            new Dictionary<string, object>() // InitialState
        ));
    }
}
