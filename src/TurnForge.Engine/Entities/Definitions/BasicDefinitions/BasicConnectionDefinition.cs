using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.BasicDefinitions;

public class BasicConnectionDefinition(string definitionId,Category category):ConnectionDefinition(definitionId,category)
{
    public new const string DefinitionId ="__BASIC_CONNECTION_DEFINITION__";
    public BasicConnectionDefinition():this(DefinitionId,Connection.ConnectionCategory)
    {
    }
}