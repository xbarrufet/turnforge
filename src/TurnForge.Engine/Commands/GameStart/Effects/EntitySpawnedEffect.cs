using TurnForge.Engine.Entities.Appliers.Results;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.GameStart.Effects;

/// <summary>
/// Effect generated when an entity is spawned.
/// Contains metadata about the spawned entity.
/// </summary>
public sealed record EntitySpawnedEffect : GameEffect
{
    public EntityId EntityId { get; init; }
    public string DefinitionId { get; init; }
    public string EntityType { get; init; }
    public string Category { get; init; }
    public Position Position { get; init; }

    public override string Description => 
        $"{EntityType} '{DefinitionId}' ({Category}) spawned at {Position}";

    public EntitySpawnedEffect(
        EntityId entityId,
        string definitionId,
        string entityType,
        string category,
        Position position,
        EffectOrigin origin = EffectOrigin.Command)
        : base(origin)
    {
       EntityId = entityId;
        DefinitionId = definitionId;
        EntityType = entityType;
        Category = category;
        Position = position;
    }
}
