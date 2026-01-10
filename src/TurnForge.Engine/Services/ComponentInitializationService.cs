using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Services;

/// <summary>
/// Service responsible for initializing components on game entities based on their traits.
/// Supports both automatic discovery (via reflection) and custom factory registration.
/// </summary>
public class ComponentInitializationService
{
    // Automatic discovery: Trait Type -> Component Type
    private Dictionary<Type, Type> _discoveredMap = new();
    
    // Custom factories: Trait Type -> Factory Function
    private readonly Dictionary<Type, Func<ITrait, IGameEntityComponent>> _customFactories = new();

    /// <summary>
    /// Initializes the service and discovers all Trait->Component mappings via reflection.
    /// </summary>
    public ComponentInitializationService()
    {
        InitializeDiscovery();
    }

    /// <summary>
    /// Discovers all components that have a constructor accepting a single ITrait parameter.
    /// This provides automatic Trait->Component mapping without manual registration.
    /// </summary>
    private void InitializeDiscovery()
    {
        _discoveredMap = DiscoverTraitConstructors();
    }

    /// <summary>
    /// Scans all loaded assemblies to find components with constructors that accept a single ITrait parameter.
    /// </summary>
    /// <returns>Dictionary mapping Trait types to their corresponding Component types.</returns>
    private Dictionary<Type, Type> DiscoverTraitConstructors()
    {
        var map = new Dictionary<Type, Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var componentTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IGameEntityComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var componentType in componentTypes)
        {
            // Look for constructors with a single ITrait parameter
            foreach (var constructor in componentType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length == 1 &&
                    typeof(ITrait).IsAssignableFrom(parameters[0].ParameterType))
                {
                    var traitType = parameters[0].ParameterType;
                    map[traitType] = componentType;
                }
            }
        }
        return map;
    }

    /// <summary>
    /// Registers a custom factory for creating components from a specific trait type.
    /// Custom factories take precedence over automatic discovery.
    /// </summary>
    /// <typeparam name="TTrait">The trait type to register a factory for.</typeparam>
    /// <param name="factory">Function that creates a component from the trait.</param>
    /// <example>
    /// <code>
    /// service.RegisterCustomFactory&lt;VitalityTrait&gt;(trait => 
    ///     new CustomHealthComponent(trait)
    /// );
    /// </code>
    /// </example>
    public void RegisterCustomFactory<TTrait>(Func<TTrait, IGameEntityComponent> factory) 
        where TTrait : ITrait
    {
        _customFactories[typeof(TTrait)] = trait => factory((TTrait)trait);
    }

    /// <summary>
    /// Unregisters a custom factory for a specific trait type.
    /// After unregistering, the service will fall back to automatic discovery.
    /// </summary>
    /// <typeparam name="TTrait">The trait type to unregister.</typeparam>
    /// <returns>True if a factory was removed, false if no factory was registered.</returns>
    public bool UnregisterCustomFactory<TTrait>() where TTrait : ITrait
    {
        return _customFactories.Remove(typeof(TTrait));
    }

    /// <summary>
    /// Checks if a custom factory is registered for a specific trait type.
    /// </summary>
    /// <typeparam name="TTrait">The trait type to check.</typeparam>
    /// <returns>True if a custom factory is registered, false otherwise.</returns>
    public bool HasCustomFactory<TTrait>() where TTrait : ITrait
    {
        return _customFactories.ContainsKey(typeof(TTrait));
    }

    /// <summary>
    /// Initializes all components on an entity based on its traits.
    /// Custom factories are prioritized over automatic discovery.
    /// </summary>
    /// <param name="entity">The entity to initialize components for.</param>
    public void InitializeComponents(GameEntity entity)
    {
        var traits = entity.GetAllTraits();
        if (!traits.Any()) return;

        foreach (var trait in traits)
        {
            var traitType = trait.GetType();
            
            // Priority 1: Custom factories
            if (_customFactories.TryGetValue(traitType, out var customFactory))
            {
                try
                {
                    var component = customFactory(trait);
                    if (component != null)
                    {
                        entity.ReplaceComponent(component);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error creating component via custom factory for trait {traitType.Name}: {e.Message}");
                }
                continue;
            }
            
            // Priority 2: Automatic discovery
            if (_discoveredMap.TryGetValue(traitType, out var componentType))
            {
                try
                {
                    var component = Activator.CreateInstance(componentType, trait) as IGameEntityComponent;
                    if (component != null)
                    {
                        entity.ReplaceComponent(component);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error creating component via discovery for trait {traitType.Name}: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Gets the number of custom factories currently registered.
    /// </summary>
    public int CustomFactoryCount => _customFactories.Count;

    /// <summary>
    /// Gets the number of automatically discovered Trait->Component mappings.
    /// </summary>
    public int DiscoveredMappingCount => _discoveredMap.Count;
}
