// csharp
using System.Collections.Generic;
using TurnForge.Engine.Entities.Actors.Components;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class Unit : Agent
{
    public UnitDefinition Definition { get; }

    public Unit(
        ActorId id,
        Position position,
        UnitDefinition definition,
        IReadOnlyList<IActorBehaviour>? behaviours = null)
        : base(id, position, new HealthComponent(definition.MaxHealth),behaviours)
    {
        Definition = definition;
    }
}