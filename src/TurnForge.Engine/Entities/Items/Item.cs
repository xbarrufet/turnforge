using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Items;

/// <summary>
/// Represents an item in the game (weapon, armor, key, etc.).
/// </summary>
/// <remarks>
/// Items are GameEntities but do NOT have a PositionComponent.
/// They always belong to an owner (Agent or Container).
/// 
/// Key components:
/// - ItemComponent: ownership and category
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
