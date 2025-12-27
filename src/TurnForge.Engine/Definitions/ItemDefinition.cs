namespace TurnForge.Engine.Definitions;

/// <summary>
/// Definition template for Items. Loaded from JSON/data files.
/// </summary>
/// <remarks>
/// Contains static data that defines what an item "is".
/// Runtime instances are created as Item entities.
/// </remarks>
public class ItemDefinition : BaseGameEntityDefinition
{
    public ItemDefinition(string definitionId, string category):base(definitionId, category)
    {

    }
}
