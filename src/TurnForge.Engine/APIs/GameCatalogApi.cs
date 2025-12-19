using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Services.Interfaces;

namespace TurnForge.Engine.APIs;

public class GameCatalogApi:IGameCatalogApi
{
    IGameCatalog _catalog;
    public GameCatalogApi(IGameCatalog catalog)
    {
        _catalog = catalog;
    }

    public void RegiterNpcDefinition(NpcTypeId typeId, NpcDefinition definition)
    {
        _catalog.RegiterNpcDefinition(typeId, definition);
    }

    public void RegisterUnitDefinition(UnitTypeId typeId, UnitDefinition definition)
    {
        _catalog.RegisterUnitDefinition(typeId, definition);
    }

    public void RegisterPropDefinition(PropTypeId typeId, PropDefinition definition)
    {
        _catalog.RegisterPropDefinition(typeId, definition);
    }

    public NpcDefinition GetNpcDefinition(NpcTypeId typeId)
    {
        return _catalog.GetNpcDefinition(typeId);
    }

    public UnitDefinition GetUnitDefinition(UnitTypeId typeId)
    {
        return _catalog.GetUnitDefinition(typeId);
    }

    public PropDefinition GetPropDefinition(PropTypeId typeId)
    {
        return _catalog.GetPropDefinition(typeId);
    }
}