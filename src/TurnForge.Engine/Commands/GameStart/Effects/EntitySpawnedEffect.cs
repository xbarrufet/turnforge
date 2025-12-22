using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.GameStart.Effects;

/// <summary>
/// Effect generated when an entity is spawned.
/// Contains metadata about the spawned entity.
/// </summary>
public sealed record EntitySpawnedEffect : IGameEffect
{
    public EntityId EntityId { get; init; }
    public string DefinitionId { get; init; }
    public string EntityType { get; init; }
    public string Category { get; init; }
    public Position Position { get; init; }

    public EntitySpawnedEffect(
        EntityId entityId,
        string definitionId,
        string entityType,
        string category,
        Position position)
    {
       EntityId = entityId;
        DefinitionId = definitionId;
        EntityType = entityType;
        Category = category;
        Position = position;
    }
}
