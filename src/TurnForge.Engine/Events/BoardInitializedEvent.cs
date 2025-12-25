using TurnForge.Engine.Appliers.Entity.Results;

namespace TurnForge.Engine.Events;

/// <summary>
/// Event generated when the game board is initialized.
/// Contains metadata about the board for UI/logging.
/// </summary>
public sealed record BoardInitializedEvent : GameEvent
{
    public int ZoneCount { get; init; }
    public string SpatialModelType { get; init; }
    
    public override string Description => 
        $"Board initialized with {ZoneCount} zones using {SpatialModelType}";
    
    public BoardInitializedEvent(
        int zoneCount,
        string spatialModelType,
        EventOrigin origin = EventOrigin.Command)
        : base(origin)
    {
        ZoneCount = zoneCount;
        SpatialModelType = spatialModelType;
    }
}
