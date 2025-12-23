using TurnForge.Engine.Entities.Appliers.Results;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;

namespace TurnForge.Engine.Entities.Effects.Board;

/// <summary>
/// Effect generated when the game board is initialized.
/// Contains metadata about the board for UI/logging.
/// </summary>
public sealed record BoardInitializedEffect : GameEffect
{
    public int ZoneCount { get; init; }
    public string SpatialModelType { get; init; }
    
    public override string Description => 
        $"Board initialized with {ZoneCount} zones using {SpatialModelType}";
    
    public BoardInitializedEffect(
        int zoneCount,
        string spatialModelType,
        EffectOrigin origin = EffectOrigin.Command)
        : base(origin)
    {
        ZoneCount = zoneCount;
        SpatialModelType = spatialModelType;
    }
}
