using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components;

/// <summary>
/// Storage component for items. Used by both Agents and Containers (Props).
/// </summary>
/// <remarks>
/// Items in this list are NOT equipped - they are just carried/stored.
/// For equipped items, see EquipmentComponent.
/// </remarks>
public class InventoryComponent : IGameEntityComponent
{
    private readonly List<EntityId> _items = new();
    
    /// <summary>
    /// List of item EntityIds stored in this inventory.
    /// </summary>
    public IReadOnlyList<EntityId> Items => _items;
    
    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    public void Add(EntityId itemId)
    {
        if (!_items.Contains(itemId))
        {
            _items.Add(itemId);
        }
    }
    
    /// <summary>
    /// Removes an item from the inventory.
    /// </summary>
    /// <returns>True if item was found and removed, false otherwise.</returns>
    public bool Remove(EntityId itemId)
    {
        return _items.Remove(itemId);
    }
    
    /// <summary>
    /// Checks if an item is in this inventory.
    /// </summary>
    public bool Contains(EntityId itemId)
    {
        return _items.Contains(itemId);
    }
    
    /// <summary>
    /// Gets the count of items in inventory.
    /// </summary>
    public int Count => _items.Count;
    
    /// <summary>
    /// Creates an empty inventory component.
    /// </summary>
    public static InventoryComponent Empty() => new();
}
