using TurnForge.Engine.Entities;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.APIs.Interfaces;

namespace TurnForge.Engine.APIs;

public class GameCatalogApi : IGameCatalogApi
{
    IGameCatalog _catalog;
    
    public GameCatalogApi(IGameCatalog catalog)
    {
        _catalog = catalog;
    }

    public T GetDefinition<T>(string definitionId) where T : GameEntityDefinition
    {
        return _catalog.GetDefinition<T>(definitionId);
    }

    
    public void RegisterDefinition<T>(string definitionId, T definition) where T:GameEntityDefinition
    {
        _catalog.RegisterDefinition(definitionId, definition);
    }


    public bool TryGetDefinition<T>(string definitionId, out T definition) where T : GameEntityDefinition
    {
        return _catalog.TryGetDefinition(definitionId, out definition);
    }

    public IEnumerable<T> GetAllDefinitions<T>() where T : GameEntityDefinition
    {
        return _catalog.GetAllDefinitions<T>();
    }
}

