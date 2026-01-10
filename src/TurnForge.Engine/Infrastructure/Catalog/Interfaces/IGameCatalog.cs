using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Definitions.BasicDefinitions;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.Entities.Definitions.CoreBase;
using TurnForge.Engine.Registration;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Catalog.Interfaces;

public abstract class IGameCatalog
{

    public abstract void RegisterDefinition<T>(T definition) where T : BaseGameEntityDefinition;

    public void RegisterDefinition<T>(string definitionId, Category category) where T : BaseGameEntityDefinition
    {
        var type = typeof(T);

        // Check most specific types first, then base type
        if (type == typeof(AgentDefinition))
            RegisterDefinition((new BasicAgentDefinition(definitionId, category) as T)!);
        else if (type == typeof(PropDefinition))
            RegisterDefinition((new BasicPropDefinition(definitionId, category) as T)!);
        else if (type == typeof(ConnectionDefinition))
            RegisterDefinition((new BasicConnectionDefinition(definitionId, category) as T)!);
        else if (type == typeof(ZoneDefinition))
            RegisterDefinition((new BasicZoneDefinition(definitionId, category) as T)!);
        else if (type == typeof(BaseGameEntityDefinition))
            RegisterDefinition((new BaseGameEntityDefinition(definitionId, category) as T)!);
        else
            throw new NotSupportedException($"Registration of type {type.Name} is not supported.");
    }
    public abstract T GetDefinition<T>(string definitionId) where T : BaseGameEntityDefinition;
    public abstract bool TryGetDefinition<T>(string definitionId, out T definition) where T : BaseGameEntityDefinition;
    public abstract IEnumerable<T> GetAllDefinitions<T>() where T : BaseGameEntityDefinition;

    public IGameCatalog()
    {
        _registerBasicDefinitions();
    }

    private void _registerBasicDefinitions()
    {
        RegisterDefinition<AgentDefinition>(new BasicAgentDefinition());
        RegisterDefinition<PropDefinition>(new BasicPropDefinition());
        RegisterDefinition<ConnectionDefinition>(new BasicConnectionDefinition());
        RegisterDefinition<ZoneDefinition>(new BasicZoneDefinition());
    }
}