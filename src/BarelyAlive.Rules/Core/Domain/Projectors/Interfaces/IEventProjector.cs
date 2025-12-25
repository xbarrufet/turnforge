using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using TurnForge.Engine.Events; // Updated namespace
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;

public interface IEventProjector
{
    bool CanHandle(IGameEvent gameEvent);
    void Project(IGameEvent gameEvent, ICollection<EntityBuildUpdate> created, ICollection<EntityStateUpdate> updated, ICollection<DomainEvent> events);
}
