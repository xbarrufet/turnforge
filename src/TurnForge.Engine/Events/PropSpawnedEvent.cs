using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Events;

public sealed record PropSpawnedEvent : GameEvent
{
    public EntityId PropId { get; init; }
    public string DefinitionId { get; init; }
    public Position Position { get; init; }

    public override string Description => $"Prop '{DefinitionId}' spawned at {Position}";

    public PropSpawnedEvent(
        EntityId propId,
        string definitionId,
        Position position,
        EventOrigin origin = EventOrigin.Command)
        : base(origin)
    {
        PropId = propId;
        DefinitionId = definitionId;
        Position = position;
    }
}
