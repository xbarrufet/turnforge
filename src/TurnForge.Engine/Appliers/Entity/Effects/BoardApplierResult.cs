using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Appliers.Entity.Effects;

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