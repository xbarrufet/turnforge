using System.Collections.Generic;
using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.Definitions.Board.Interfaces; // Assuming ZoneDefinition needs this or similar
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Events;

namespace TurnForge.Engine.Events;

public sealed record BoardCreatedEvent : GameEvent
{
    public EntityId BoardId { get; init; }
    public IReadOnlyList<ZoneDefinition> Zones { get; init; }

    public override string Description => $"Board created with {Zones.Count} zones";

    public BoardCreatedEvent(
        EntityId boardId,
        IReadOnlyList<ZoneDefinition> zones,
        EventOrigin origin = EventOrigin.Command)
        : base(origin)
    {
        BoardId = boardId;
        Zones = zones;
    }
}
