using TurnForge.Engine.Definitions;
using TurnForge.Engine.Registration;

namespace TurnForge.Engine.Infrastructure.Catalog.Interfaces;

public interface IGameCatalog
{
    
    void RegisterDefinition<T>(T definition) where T:BaseGameEntityDefinition;
    void RegisterDefinition(string definitionId, string category);
    T GetDefinition<T>(string definitionId) where T : BaseGameEntityDefinition;
    bool TryGetDefinition<T>(string definitionId, out T? definition) where T : BaseGameEntityDefinition;
    IEnumerable<T> GetAllDefinitions<T>() where T : BaseGameEntityDefinition;
}