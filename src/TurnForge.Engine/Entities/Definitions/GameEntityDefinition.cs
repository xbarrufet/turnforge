using TurnForge.Engine.Behaviours.Interfaces;

namespace TurnForge.Engine.Entities;

public class BaseGameEntityDefinition
{

    public BaseGameEntityDefinition() {

    }

    public BaseGameEntityDefinition(string definitionId, string name, string category) {
        DefinitionId = definitionId;
        Name = name;
        Category = category;
    }
    
    public string DefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Dictionary of dynamic attributes (e.g., "Strength": 5, "Damage": "1D6").
    /// Values can be int (simple stat) or string (dice notation).
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; } = new();

}