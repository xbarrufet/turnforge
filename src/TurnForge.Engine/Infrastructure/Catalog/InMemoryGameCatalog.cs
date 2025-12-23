    using BarelyAlive.Rules.Registration;
    using TurnForge.Engine.Entities;
    using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
    using TurnForge.Engine.Registration;

    namespace TurnForge.Engine.Infrastructure.Catalog;

    internal sealed class InMemoryGameCatalog : IGameCatalog
    {
        public IDefinitionRegistry<string, GameEntityDefinition> Entities { get; } = new InMemoryDefinitionRegistry<string, GameEntityDefinition>();

        

        public T GetDefinition<T>(string definitionId) where T : GameEntityDefinition
        {
            return Entities.Get(definitionId) as T;
        }

        public bool TryGetDefinition<T>(string definitionId, out T? definition) where T : GameEntityDefinition
        {
            try
            {
                var entity = Entities.Get(definitionId);
                if (entity is T typedEntity)
                {
                    definition = typedEntity;
                    return true;
                }
                definition = null;
                return false;
            }
            catch
            {
                definition = null;
                return false;
            }
        }

        public IEnumerable<T> GetAllDefinitions<T>() where T : GameEntityDefinition
        {
            return Entities.GetAll().OfType<T>();
        }

        public void RegisterDefinition<T>(string definitionId, T definition) where T : GameEntityDefinition
        {
            Entities.Register(definitionId, definition);
        }

        public InMemoryGameCatalog()
        {
        }

    }