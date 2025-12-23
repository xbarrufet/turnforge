using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;

public interface IEffectProjector
{
    bool CanHandle(IGameEffect effect);
    void Project(IGameEffect effect, ICollection<EntityBuildUpdate> created, ICollection<EntityStateUpdate> updated, ICollection<DomainEvent> events);
}
