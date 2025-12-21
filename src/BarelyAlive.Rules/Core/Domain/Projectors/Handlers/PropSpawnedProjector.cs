using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using BarelyAlive.Rules.Core.Domain.ValueObjects;
using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Handlers;

public class PropSpawnedProjector : IEffectProjector
{
    public bool CanHandle(IGameEffect effect) => effect is PropSpawnedEffect;

    public void Project(IGameEffect effect, ICollection<EntityBuildUpdate> created, ICollection<EntityStateUpdate> updated, ICollection<DomainEvent> events)
    {
        if (effect is not PropSpawnedEffect prop) return;

        created.Add(new EntityBuildUpdate(
            prop.PropId.Value.ToString(),
            "Prop",
            prop.PropType.Value,
            new Vector(prop.Position.X, prop.Position.Y),
            new Dictionary<string, object>()
        ));
    }
}
