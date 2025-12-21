namespace BarelyAlive.Rules.Apis.Messaging;

using System.Collections.Generic;




public record GameUpdatePayload(
    List<EntityBuildUpdate> Created,
    List<EntityStateUpdate> Updated,
    List<DomainEvent> Events
);
