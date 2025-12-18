// csharp
using System.Collections.Generic;
using TurnForge.Engine.Entities.Actors.Components;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class Npc : Agent
{
    public NpcDefinition Definition { get; }

    public Npc(
        ActorId id,
        Position position,
        NpcDefinition definition,
        IReadOnlyList<IActorBehaviour>? behaviours = null)
        : base(id, position, new HealthComponent(definition.MaxHealth), behaviours)
    {
        Definition = definition;
    }
}