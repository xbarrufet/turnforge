using TurnForge.Engine.Definitions;
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

    public T GetDefinition<T>(string definitionId) where T : BaseGameEntityDefinition
    {
        return _catalog.GetDefinition<T>(definitionId);
    }

    
    public void RegisterDefinition<T>(T definition) where T:BaseGameEntityDefinition
    {
        _catalog.RegisterDefinition( definition);
    }


    public bool TryGetDefinition<T>(string definitionId, out T definition) where T : BaseGameEntityDefinition
    {
        return _catalog.TryGetDefinition(definitionId, out definition);
    }

    public IEnumerable<T> GetAllDefinitions<T>() where T : BaseGameEntityDefinition
    {
        return _catalog.GetAllDefinitions<T>();
    }

    public void RegisterDefinition(string definitionId, string category)
    {
        _catalog.RegisterDefinition(definitionId, category);
    }
}

