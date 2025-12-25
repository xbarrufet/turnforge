using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using BarelyAlive.Rules.Core.Domain.ValueObjects;
using TurnForge.Engine.Events;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Handlers;

public class PropSpawnedProjector : IEventProjector
{
    public bool CanHandle(IGameEvent gameEvent) => gameEvent is PropSpawnedEvent;

    public void Project(IGameEvent gameEvent, ICollection<EntityBuildUpdate> created, ICollection<EntityStateUpdate> updated, ICollection<DomainEvent> events)
    {
        if (gameEvent is not PropSpawnedEvent prop) return;
        
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
