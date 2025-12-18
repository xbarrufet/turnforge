// csharp
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities.Actors.Components;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public class Prop : Actor
{
    public PropDefinition Definition { get; }
    public HealthComponent? Health { get; }

    public bool IsDestructible => Health != null;

    public Prop(
        ActorId id,
        Position position,
        PropDefinition definition,
        HealthComponent? health = null,
        IReadOnlyList<IActorBehaviour>? behaviours = null)
        : base(id, position, behaviours)
    {
        Definition = definition;
        Health = health;
    }
}