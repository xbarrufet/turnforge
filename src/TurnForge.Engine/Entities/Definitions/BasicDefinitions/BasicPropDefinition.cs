using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.BasicDefinitions;

public class BasicPropDefinition(string definitionId,Category category):PropDefinition(definitionId,category)
{
        public new const string DefinitionId ="__BASIC_PROP_DEFINITION__";
        
        public BasicPropDefinition():this(DefinitionId,Prop.PropDefaultCategory)
        {
        }
        
}