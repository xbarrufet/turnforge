using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Appliers.Entity.Effects;

public sealed record PropSpawnedEffect : GameEffect
{
    public EntityId PropId { get; init; }
    public string DefinitionId { get; init; }
    public Position Position { get; init; }

    public override string Description => $"Prop '{DefinitionId}' spawned at {Position}";

    public PropSpawnedEffect(
        EntityId propId,
        string definitionId,
        Position position,
        EffectOrigin origin = EffectOrigin.Command)
        : base(origin)
    {
        PropId = propId;
        DefinitionId = definitionId;
        Position = position;
    }
}
