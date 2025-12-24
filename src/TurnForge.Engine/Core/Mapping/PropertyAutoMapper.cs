using System.Reflection;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Behaviours.Interfaces;

namespace TurnForge.Engine.Core.Mapping;

/// <summary>
/// Service responsible for automatically mapping properties from a source object 
/// (like a Descriptor or Definition) to the Entity's Components.
/// Supports Implicit Mapping (via Registry) and Explicit Mapping (via Attributes).
/// </summary>
public static class PropertyAutoMapper
{
    private static readonly MethodInfo _mapMethod = typeof(PropertyAutoMapper)
        .GetMethod(nameof(Map), BindingFlags.Public | BindingFlags.Static);

    /// <summary>
    /// Maps properties from the source object to the target entity's components.
    /// </summary>
    /// <param name="source">The source object (e.g., PropDescriptor, AgentDefinition).</param>
    /// <param name="target">The target entity to populate.</param>
    public static void Map(object source, GameEntity target)
    {
        if (source == null || target == null) return;

        var sourceType = source.GetType();
        var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        // 1. Get all components of the entity
        // We need to iterate over components to see what can be mapped
        // Note: Generic access to components is tricky on GameEntity unless we use GetAllComponents
        // For now, we rely on the Registry to know what properties exist on what component types
        // and we check if the entity has that component type.

        foreach (var sourceProp in sourceProperties)
        {
            // 0. Handle [MapToBehaviours] (Special Case)
            if (sourceProp.GetCustomAttribute<MapToBehavioursAttribute>() != null)
            {
                MapBehaviours(source, target, sourceProp);
                continue;
            }

            // 1. Handle Explicit [MapToComponent]
            var explicitAttributes = sourceProp.GetCustomAttributes<MapToComponentAttribute>();
            if (explicitAttributes.Any())
            {
                foreach (var attr in explicitAttributes)
                {
                    MapExplicitly(source, target, sourceProp, attr);
                }
                // If explicit attributes exist, we typically skip implicit mapping for this property
                // unless we want dual behavior. Conventionally, explicit overrides implicit.
                continue;
            }

            // 2. Handle Implicit Mapping (Convention based)
            MapImplicitly(source, target, sourceProp);
        }
    }

    private static void MapImplicitly(object source, GameEntity target, PropertyInfo sourceProp)
    {
        // We don't know which component matches this property yet.
        // Efficient approach:
        // We could iterate all components on the entity, and for each, check if it has a setter for PropName.
        
        // But GameEntity doesn't expose GetAllComponents generic list easily without allocation.
        // Ideally, we iterate the known components of the target.
        // Since we can't easily iterate target.Components generically without casting,
        // we might need to rely on the target exposing them or iterating registered types.
        
        // Alternative: The ComponentSetterRegistry stores Map<ComponentType, ...>
        // We can check if the current SourceProp name exists in ANY component.
        
        // Allow the entity to provide its components
        // Assuming GameEntity has a way to iterate components, if not we might need to rely on
        // checking the Registry for "What components COULD have this property" -> "Does Entity have this Component?"
        
        // For now, let's assume we scan the Entity's attached components.
        // Since GameEntity implementations might vary, we might need a tighter loop.
        // Let's rely on the Registry to find candidate ComponentTypes for this PropertyName.
        // (This would require a reverse lookup: PropertyName -> List<ComponentType>)
        // OR
        // Iterate entity components.

        if (target is IComponentContainer container)
        {
             foreach (var component in container.GetAllComponents())
             {
                 MapToComponent(source, component, sourceProp);
             }
        }
    }

    private static void MapToComponent(object source, IGameEntityComponent component, PropertyInfo sourceProp)
    {
        var componentType = component.GetType();
        var settableProps = ComponentSetterRegistry.GetSettableProperties(componentType);

        if (settableProps.TryGetValue(sourceProp.Name, out var targetProp))
        {
            // Type Check
            if (targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
            {
                try 
                {
                    var value = sourceProp.GetValue(source);
                    targetProp.SetValue(component, value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PropertyAutoMapper] Failed to map {sourceProp.Name} to {componentType.Name}: {ex.Message}");
                }
            }
        }
    }

    private static void MapExplicitly(object source, GameEntity target, PropertyInfo sourceProp, MapToComponentAttribute attr)
    {
        var component = target.GetComponent(attr.ComponentType);
        if (component == null) return;

        var targetPropName = attr.PropertyName ?? sourceProp.Name;
        var componentType = component.GetType();
        var targetProp = componentType.GetProperty(targetPropName);

        if (targetProp == null || !targetProp.CanWrite) return;

        // Get value from source
        var value = sourceProp.GetValue(source);
        
        // Handle null values
        if (value == null) return;

        // Get the actual value type (unwrap nullable if needed)
        var valueType = value.GetType();
        
        // Check type compatibility using the actual value type
        if (!targetProp.PropertyType.IsAssignableFrom(valueType)) return;

        targetProp.SetValue(component, value);
    }

    private static void MapBehaviours(object source, GameEntity target, PropertyInfo sourceProperty)
    {
        // Re-using logic from original EngineAutoMapper
         var value = sourceProperty.GetValue(source);
        if (value == null) return;

        var behaviourComponent = target.GetComponent<IBehaviourComponent>();
        if (behaviourComponent == null) return;

        if (value is IEnumerable<IBaseBehaviour> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                behaviourComponent.AddBehaviour(behaviour);
            }
        }
    }
}


