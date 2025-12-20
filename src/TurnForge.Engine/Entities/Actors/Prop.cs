// csharp
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class Prop : Actor
{
    public PropDefinition Definition { get; }

    public Prop(
        EntityId id,
        PropDefinition definition,
        PositionComponent position,
        BehaviourComponent behaviourComponent) : base(id, position, behaviourComponent)
    {
        Definition = definition;
    }
}