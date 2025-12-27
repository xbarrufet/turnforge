using TurnForge.Engine.Traits.Interfaces;

namespace TurnForge.Engine.Entities;

public class BaseGameEntityDefinition
{

    public BaseGameEntityDefinition() {

    }

    public BaseGameEntityDefinition(string definitionId) {
        DefinitionId = definitionId;
    }
    
    public string DefinitionId { get; set; } = string.Empty;


    public List<IBaseTrait> Traits { get; set; } = new();

}