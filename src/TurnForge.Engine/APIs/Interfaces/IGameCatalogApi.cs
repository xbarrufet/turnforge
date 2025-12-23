using TurnForge.Engine.Entities;

namespace TurnForge.Engine.APIs.Interfaces;

public interface IGameCatalogApi
{
    void RegisterDefinition<T>( T definition) where T : BaseGameEntityDefinition;
    T GetDefinition<T>(string definitionId) where T : BaseGameEntityDefinition;
    bool TryGetDefinition<T>(string definitionId, out T? definition) where T : BaseGameEntityDefinition;
    IEnumerable<T> GetAllDefinitions<T>() where T : BaseGameEntityDefinition;
    void RegisterDefinition(string definitionId, string name, string category);
}