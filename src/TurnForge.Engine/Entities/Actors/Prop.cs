// csharp
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public class Prop(
    ActorId id,
    Position position,
    IReadOnlyList<IActorTrait>? traits = null,
    string? customType = "Prop") : Actor(id, position, traits, customType)
{
    public Prop(PropDescriptor propDescriptor, Position position)
        : this(
            propDescriptor.Id,
            position,
            ActorTraitConverter.ToTraits(propDescriptor.Traits),
            propDescriptor.CustomType)
    { }
}