using TurnForge.Engine.Entities;

namespace TurnForge.Engine.APIs.Interfaces;

public interface IGameCatalogApi
{
    void RegisterDefinition<T>(string definitionId, T definition) where T : GameEntityDefinition;
    T GetDefinition<T>(string definitionId) where T : GameEntityDefinition;
    bool TryGetDefinition<T>(string definitionId, out T? definition) where T : GameEntityDefinition;
    IEnumerable<T> GetAllDefinitions<T>() where T : GameEntityDefinition;
}