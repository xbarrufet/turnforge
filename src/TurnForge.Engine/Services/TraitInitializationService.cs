using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Traits.Interfaces;


namespace TurnForge.Engine.Services;

public class TraitInitializationService
{

    private  Dictionary<Type,Type> _traitToComponentMap;

    public TraitInitializationService() // CONSTRUCTOR
{
    InitializeTraits();
}

    public void InitializeTraits()
    {
        _traitToComponentMap = DiscoverTraitConstructors();
    }

    private Dictionary<Type,Type> DiscoverTraitConstructors()
    {
            var map = new Dictionary<Type,Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            var componentTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IGameEntityComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            
            foreach (var componentType in componentTypes)
            {
                // busca el construcutors i agadem el que tingui IBaseTrait com parametre
                foreach (var constructor in componentType.GetConstructors())
                {
                    var parameters = constructor.GetParameters();
                    if (parameters.Length == 1 && 
                        typeof(IBaseTrait).IsAssignableFrom(parameters[0].ParameterType))
                    {
                       var traitType = parameters[0].ParameterType; // El tipus del paràmetre (ex: VitalityTrait)
                       map[traitType] = componentType;              // Clau: Trait -> Valor: Component
                    }
                }   
            }
            return map;
    }       

    public void InitializeComponents(GameEntity entity) {
        var traitsContainer = entity.GetComponent<ITraitContainerComponent>();
        if(traitsContainer==null) return;

        foreach (var trait in traitsContainer.Traits)
        {
            var traitType = trait.GetType();
            // si tenum un component mapejat per aquest tipus de trait
            if (_traitToComponentMap.TryGetValue(traitType, out var componentType))
            {
                try {
                    var component =(IGameEntityComponent) Activator.CreateInstance(componentType, trait);
                    entity.ReplaceComponent(component);
                } catch (Exception e) {
                    Console.WriteLine($"Error creating component for trait {traitType.Name}: {e.Message}");
                }
            }
        }
        
    }
}
