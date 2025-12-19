using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Services.Interfaces;

public interface IGameCatalogApi
{
    void RegisterAgentDefinition(AgentTypeId typeId, AgentDefinition definition);
    void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition);

    AgentDefinition GetAgentDefinition(AgentTypeId typeId);
    PropDefinition GetPropDefinition(PropTypeId typeId);

}