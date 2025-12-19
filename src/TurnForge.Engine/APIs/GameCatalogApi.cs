using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Services.Interfaces;

namespace TurnForge.Engine.APIs;

public class GameCatalogApi : IGameCatalogApi
{
    IGameCatalog _catalog;
    public GameCatalogApi(IGameCatalog catalog)
    {
        _catalog = catalog;
    }

    public void RegisterAgentDefinition(AgentTypeId typeId, AgentDefinition definition)
    {
        _catalog.RegisterAgentDefinition(typeId, definition);
    }

    public void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition)
    {
        _catalog.RegisterPropDefinition(typeId, definition);
    }

    public AgentDefinition GetAgentDefinition(AgentTypeId typeId)
    {
        return _catalog.GetAgentDefinition(typeId);
    }

    public PropDefinition GetPropDefinition(PropTypeId typeId)
    {
        return _catalog.GetPropDefinition(typeId);
    }
}