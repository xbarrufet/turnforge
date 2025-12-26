using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components;

/// <summary>
/// Component that tracks item ownership and category.
/// </summary>
/// <remarks>
/// Every Item has this component to know:
/// - Who owns it (Agent or Container)
/// - What type of item it is (Weapon, Armor, Key...)
/// </remarks>
public class ItemComponent : IGameEntityComponent
{
    /// <summary>
    /// The EntityId of the owner (Agent or Container).
    /// Null if the item is orphaned (should not happen in normal gameplay).
    /// </summary>
    public EntityId? OwnerId { get; set; }
    
    /// <summary>
    /// The category/type of this item (e.g., "Weapon", "Armor", "Key", "Consumable").
    /// Used by strategies to determine how the item can be used.
    /// </summary>
    public string Category { get; init; } = string.Empty;
    
    public ItemComponent() { }
    
    public ItemComponent(string category, EntityId? ownerId = null)
    {
        Category = category;
        OwnerId = ownerId;
    }
    
    /// <summary>
    /// Creates a copy with a new owner.
    /// </summary>
    public ItemComponent WithOwner(EntityId ownerId) => new(Category, ownerId);
    
    /// <summary>
    /// Creates a copy with no owner (orphaned).
    /// </summary>
    public ItemComponent WithoutOwner() => new(Category, null);
}
