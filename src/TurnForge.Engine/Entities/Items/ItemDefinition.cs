namespace TurnForge.Engine.Entities.Items;

/// <summary>
/// Definition template for Items. Loaded from JSON/data files.
/// </summary>
/// <remarks>
/// Contains static data that defines what an item "is".
/// Runtime instances are created as Item entities.
/// </remarks>
public class ItemDefinition : BaseGameEntityDefinition
{
    public ItemDefinition() { }
    
    public ItemDefinition(string definitionId, string name, string category)
        : base(definitionId, name, category)
    {
    }
    
    /// <summary>
    /// Item slot where this can be equipped (e.g., "Weapon", "Armor").
    /// Null if item cannot be equipped.
    /// </summary>
    public string? EquipSlot { get; set; }
    
    /// <summary>
    /// Whether this item can be stacked (e.g., ammo, consumables).
    /// </summary>
    public bool Stackable { get; set; } = false;
    
    /// <summary>
    /// Maximum stack size if stackable.
    /// </summary>
    public int MaxStack { get; set; } = 1;
}
