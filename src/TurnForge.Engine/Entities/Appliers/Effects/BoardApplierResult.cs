using TurnForge.Engine.Entities.Appliers.Results;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers.Effects;

public sealed record BoardApplierResult : GameEffect
{
    public EntityId BoardId { get; init; }
    public IReadOnlyList<ZoneDefinition> Zones { get; init; }

    public override string Description => $"Board created with {Zones.Count} zones";

    public BoardApplierResult(
        EntityId boardId,
        IReadOnlyList<ZoneDefinition> zones,
        EffectOrigin origin = EffectOrigin.Command)
        : base(origin)
    {
        BoardId = boardId;
        Zones = zones;
    }
}