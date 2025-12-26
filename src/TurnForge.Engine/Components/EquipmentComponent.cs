using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components;

/// <summary>
/// Equipment slots for Agents. Items here are actively equipped, not just carried.
/// </summary>
/// <remarks>
/// Design decision: Equipped items are NOT in InventoryComponent.
/// They are mutually exclusive - an item is either equipped OR in inventory.
/// 
/// Slots are string-based for flexibility. Common slots:
/// - "Weapon" - Main weapon
/// - "Armor" - Body armor
/// - "Accessory" - Ring, amulet, etc.
/// 
/// Game-specific slots can be defined as needed.
/// </remarks>
public class EquipmentComponent : IGameEntityComponent
{
    private readonly Dictionary<string, EntityId?> _slots = new();
    
    /// <summary>
    /// Gets all equipment slots and their contents.
    /// </summary>
    public IReadOnlyDictionary<string, EntityId?> Slots => _slots;
    
    /// <summary>
    /// Gets the item equipped in the specified slot.
    /// </summary>
    /// <param name="slot">Slot name (e.g., "Weapon", "Armor")</param>
    /// <returns>EntityId of equipped item, or null if slot is empty.</returns>
    public EntityId? GetEquipped(string slot)
    {
        return _slots.TryGetValue(slot, out var itemId) ? itemId : null;
    }
    
    /// <summary>
    /// Equips an item in the specified slot.
    /// </summary>
    /// <param name="slot">Slot name</param>
    /// <param name="itemId">Item to equip</param>
    /// <remarks>
    /// If the slot already has an item, it will be replaced.
    /// Caller is responsible for moving the old item to inventory.
    /// </remarks>
    public void Equip(string slot, EntityId itemId)
    {
        _slots[slot] = itemId;
    }
    
    /// <summary>
    /// Unequips the item from the specified slot.
    /// </summary>
    /// <param name="slot">Slot name</param>
    /// <returns>EntityId of the unequipped item, or null if slot was empty.</returns>
    public EntityId? Unequip(string slot)
    {
        if (_slots.TryGetValue(slot, out var itemId))
        {
            _slots[slot] = null;
            return itemId;
        }
        return null;
    }
    
    /// <summary>
    /// Checks if a slot has an item equipped.
    /// </summary>
    public bool HasEquipped(string slot)
    {
        return _slots.TryGetValue(slot, out var itemId) && itemId != null;
    }
    
    /// <summary>
    /// Checks if a specific item is equipped in any slot.
    /// </summary>
    public bool IsEquipped(EntityId itemId)
    {
        return _slots.Values.Any(id => id == itemId);
    }
    
    /// <summary>
    /// Gets the slot where an item is equipped.
    /// </summary>
    /// <returns>Slot name, or null if item is not equipped.</returns>
    public string? GetSlotFor(EntityId itemId)
    {
        foreach (var kvp in _slots)
        {
            if (kvp.Value == itemId)
                return kvp.Key;
        }
        return null;
    }
    
    /// <summary>
    /// Creates an empty equipment component.
    /// </summary>
    public static EquipmentComponent Empty() => new();
}
