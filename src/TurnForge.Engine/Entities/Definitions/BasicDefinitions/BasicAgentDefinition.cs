using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.CoreBase;


public class BasicAgentDefinition(string definitionId,Category category) : AgentDefinition(definitionId,category)
{
    public new const string DefinitionId ="__BASIC_AGENT_DEFINITION__";

    public BasicAgentDefinition() : this(DefinitionId, Agent.AgentDefaultCategory)
    {
    }

}