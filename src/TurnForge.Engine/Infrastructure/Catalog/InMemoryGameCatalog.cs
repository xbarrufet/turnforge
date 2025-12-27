    using BarelyAlive.Rules.Registration;
    using TurnForge.Engine.Definitions;
    using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
    using TurnForge.Engine.Registration;

    namespace TurnForge.Engine.Infrastructure.Catalog;

    internal sealed class InMemoryGameCatalog : IGameCatalog
    {
        public IDefinitionRegistry<string, BaseGameEntityDefinition> Entities { get; } = new InMemoryDefinitionRegistry<string, BaseGameEntityDefinition>();

        

        public T GetDefinition<T>(string definitionId) where T : BaseGameEntityDefinition
        {
            return Entities.Get(definitionId) as T;
        }

        public bool TryGetDefinition<T>(string definitionId, out T? definition) where T : BaseGameEntityDefinition
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

        public IEnumerable<T> GetAllDefinitions<T>() where T : BaseGameEntityDefinition
        {
            return Entities.GetAll().OfType<T>();
        }

        public void RegisterDefinition<T>( T definition) where T : BaseGameEntityDefinition
        {
            Entities.Register(definition.DefinitionId, definition);
        }

        public void RegisterDefinition(string definitionId, string category)
        {
            var def = new BaseGameEntityDefinition(definitionId, category);
            def.AddTrait(new TurnForge.Engine.Traits.Standard.IdentityTrait(category));
            Entities.Register(definitionId, def);
        }

        public InMemoryGameCatalog()
        {
        }

    }