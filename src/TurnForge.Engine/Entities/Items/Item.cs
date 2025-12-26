using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Items;

/// <summary>
/// Represents an item in the game (weapon, armor, key, etc.).
/// </summary>
/// <remarks>
/// Items are GameEntities but do NOT have a PositionComponent.
/// They always belong to an owner (Agent or Container).
/// 
/// Key components:
/// - ItemComponent: ownership and category
/// - AttributeComponent: stats (Damage, Range, etc.)
/// </remarks>
public class Item : GameEntity
{
    /// <summary>
    /// Quick access to the item's ownership and category component.
    /// </summary>
    public ItemComponent ItemComponent { get; private set; }
    
    public Item(
        EntityId id,
        string definitionId,
        string name,
        string category) : base(id, name, category, definitionId)
    {
        // Initialize with empty ItemComponent
        var itemComponent = new ItemComponent(category);
        AddComponent(itemComponent);
        ItemComponent = itemComponent;
    }
    
    /// <summary>
    /// Gets an attribute value from this item.
    /// </summary>
    /// <typeparam name="T">Type of the attribute value.</typeparam>
    /// <param name="key">Attribute name (e.g., "Damage", "Range").</param>
    /// <returns>Attribute value, or default if not found.</returns>
    public T? GetAttribute<T>(string key)
    {
        var attributes = GetComponent<AttributeComponent>();
        var value = attributes?.Get(key);
        if (value == null) return default;
        
        // For int type, return CurrentValue
        if (typeof(T) == typeof(int))
        {
            return (T)(object)value.Value.CurrentValue;
        }
        
        // For other types, try direct conversion
        return default;
    }
    
    /// <summary>
    /// Sets the owner of this item.
    /// </summary>
    public void SetOwner(EntityId ownerId)
    {
        ItemComponent = ItemComponent.WithOwner(ownerId);
        AddComponent(ItemComponent);
    }
    
    /// <summary>
    /// Clears the owner of this item.
    /// </summary>
    public void ClearOwner()
    {
        ItemComponent = ItemComponent.WithoutOwner();
        AddComponent(ItemComponent);
    }
}
