using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Infrastructure.Registration;
using TurnForge.Engine.Registration;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Catalog;

public sealed class InMemoryGameCatalog : IGameCatalog
{
    public IDefinitionRegistry<string, BaseGameEntityDefinition> Entities { get; } = new InMemoryDefinitionRegistry<string, BaseGameEntityDefinition>();




    public override T GetDefinition<T>(string definitionId)
    {
        return (Entities.Get(definitionId) as T)!;
    }



    public override bool TryGetDefinition<T>(string definitionId, out T definition)
    {
        try
        {
            var entity = Entities.Get(definitionId);
            if (entity is T typedEntity)
            {
                definition = typedEntity;
                return true;
            }
            definition = null!;
            return false;
        }
        catch
        {
            definition = null!;
            return false;
        }
    }

    public override IEnumerable<T> GetAllDefinitions<T>()
    {
        return Entities.GetAll().OfType<T>();
    }

    public override void RegisterDefinition<T>(T definition)
    {
        Entities.Register(definition.DefinitionId, definition);
    }

    public InMemoryGameCatalog() : base()
    {
    }


}