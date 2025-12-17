// csharp
using System.Collections.Generic;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public class Unit(
    ActorId id,
    Position position,
    IReadOnlyList<IActorTrait>? traits = null,
    string? customType = "Unit") : Actor(id, position, traits, customType)
{
    public Unit(UnitDescriptor unitDescriptor, Position position)
        : this(
            unitDescriptor.Id,
            position,
            ActorTraitConverter.ToTraits(unitDescriptor.Traits),
            unitDescriptor.CustomType)
    { }
}