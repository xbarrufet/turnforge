using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Appliers.Effects;

/// <summary>
/// Effect indicating that components were updated on an entity.
/// Used by UI to trigger animations/updates.
/// </summary>
public sealed record ComponentsUpdatedEffect : GameEffect
{
    public EntityId EntityId { get; init; }
    
    /// <summary>
    /// Types of components that were updated.
    /// </summary>
    public Type[] UpdatedComponentTypes { get; init; }
    
    public override string Description => 
        $"Updated {UpdatedComponentTypes.Length} component(s) on entity {EntityId}";
    
    public ComponentsUpdatedEffect(
        EntityId entityId, 
        Type[] updatedComponentTypes,
        EffectOrigin origin = EffectOrigin.Command)
        : base(origin)
    {
        EntityId = entityId;
        UpdatedComponentTypes = updatedComponentTypes ?? Array.Empty<Type>();
    }
}
