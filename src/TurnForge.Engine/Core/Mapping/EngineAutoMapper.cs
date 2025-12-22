using System.Reflection;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Behaviours.Interfaces;
using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Core.Mapping;

/// <summary>
/// Automatic mapper that transfers properties from Definitions/Descriptors to Entity Components.
/// Supports both simple property mapping via [MapToComponent] and behaviour collection mapping via [MapToBehaviours].
/// </summary>
public static class EngineAutoMapper
{
    /// <summary>
    /// Maps properties from source object to target entity's components based on attributes.
    /// </summary>
    /// <param name="source">Source object (Definition or Descriptor)</param>
    /// <param name="target">Target GameEntity to populate</param>
    public static void Map(object source, GameEntity target)
    {
        var sourceType = source.GetType();
        
        // Iterate over all public instance properties of the source
        foreach (var sourceProperty in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            // Check for [MapToBehaviours] first (special case)
            var behaviourAttribute = sourceProperty.GetCustomAttribute<MapToBehavioursAttribute>();
            if (behaviourAttribute != null)
            {
                MapBehaviours(source, target, sourceProperty);
                continue; // Skip other attributes for this property
            }
            
            // Check for [MapToComponent]
            var componentAttributes = sourceProperty.GetCustomAttributes<MapToComponentAttribute>();
            foreach (var attribute in componentAttributes)
            {
                MapProperty(source, target, sourceProperty, attribute);
            }
        }
    }

    /// <summary>
    /// Maps a behaviour collection to the entity's BehaviourComponent.
    /// Copies each behaviour to maintain isolation between source and entity.
    /// </summary>
    private static void MapBehaviours(object source, GameEntity target, PropertyInfo sourceProperty)
    {
        var value = sourceProperty.GetValue(source);
        if (value == null) return;

        // Get BehaviourComponent from entity
        var behaviourComponent = target.GetComponent<IBehaviourComponent>();
        if (behaviourComponent == null)
        {
            throw new InvalidOperationException(
                $"Entity '{target.Name}' does not have a BehaviourComponent. " +
                $"Cannot map behaviours from property '{sourceProperty.Name}'.");
        }

        // Verify property is an enumerable of IBaseBehaviour
        if (value is not IEnumerable<IBaseBehaviour> behaviours)
        {
            throw new InvalidOperationException(
                $"Property '{sourceProperty.Name}' marked with [MapToBehaviours] must be IEnumerable<IBaseBehaviour>. " +
                $"Found type: {sourceProperty.PropertyType.Name}");
        }

        // Copy each behaviour to the component
        foreach (var behaviour in behaviours)
        {
            // For now, we add the behaviour reference directly.
            // If behaviours need to be cloned, implement ICloneable on IBaseBehaviour
            behaviourComponent.AddBehaviour(behaviour);
        }
    }

    /// <summary>
    /// Maps a single property to a component's property based on MapToComponent attribute.
    /// </summary>
    private static void MapProperty(object source, GameEntity target, PropertyInfo sourceProperty, MapToComponentAttribute attribute)
    {
        // Get value from source
        var value = sourceProperty.GetValue(source);
        if (value == null) return;

        // Find component in target entity
        var component = target.GetComponent(attribute.ComponentType);
        if (component == null) return;

        // Set property value on component
        // Use concrete component type (not interface) to find writable properties
        var targetPropertyName = attribute.PropertyName ?? sourceProperty.Name;
        var componentType = component.GetType();
        var targetProperty = componentType.GetProperty(targetPropertyName);

        if (targetProperty != null && targetProperty.CanWrite)
        {
            targetProperty.SetValue(component, value);
        }
    }
}