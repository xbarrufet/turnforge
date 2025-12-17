// csharp
using System.Collections.Generic;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public class Hostile(
    ActorId id,
    Position position,
    IReadOnlyList<IActorTrait>? traits = null,
    string? customType = "Hostile") : Actor(id, position, traits, customType)
{
    public Hostile(HostileDescriptor hostileDescriptor, Position position)
        : this(
            hostileDescriptor.Id,
            position,
            ActorTraitConverter.ToTraits(hostileDescriptor.Traits),
            hostileDescriptor.CustomType)
    { }
}