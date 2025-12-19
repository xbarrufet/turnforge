using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Registration;

namespace TurnForge.Engine.Infrastructure.Catalog.Interfaces;

public interface IGameCatalog
{
    IDefinitionRegistry<PropTypeId, PropDefinition> Props { get; }
    IDefinitionRegistry<AgentTypeId, AgentDefinition> Agents { get; }

    void RegisterAgentDefinition(AgentTypeId typeId, AgentDefinition definition);
    void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition);

    AgentDefinition GetAgentDefinition(AgentTypeId typeId);
    PropDefinition GetPropDefinition(PropTypeId typeId);
}