using TurnForge.Engine.Entities;
using TurnForge.Engine.Registration;

namespace TurnForge.Engine.Infrastructure.Catalog.Interfaces;

public interface IGameCatalog
{
    
    void RegisterDefinition<T>(string definitionId, T definition) where T:GameEntityDefinition;
    T GetDefinition<T>(string definitionId) where T : GameEntityDefinition;
    bool TryGetDefinition<T>(string definitionId, out T? definition) where T : GameEntityDefinition;
    IEnumerable<T> GetAllDefinitions<T>() where T : GameEntityDefinition;
}