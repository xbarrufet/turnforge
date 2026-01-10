using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.BasicDefinitions;

public class BasicZoneDefinition(string definitionid,Category category):ZoneDefinition(definitionid)
{
    public new const string DefinitionId ="__BASIC_ZONE_DEFINITION__";
    public BasicZoneDefinition():this(DefinitionId,Zone.ZoneDefaultCategory)
    {
    }
}