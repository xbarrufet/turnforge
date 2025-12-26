using System.Reflection;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Behaviours.Interfaces;
using TurnForge.Engine.Components.Interfaces;

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
            // Check for [MapToTraits] first (special case)
            var traitAttribute = sourceProperty.GetCustomAttribute<MapToTraitsAttribute>();
            if (traitAttribute != null)
            {
                MapTraits(source, target, sourceProperty);
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
    /// Maps a trait collection to the entity's TraitContainerComponent.
    /// Copies each trait to maintain isolation between source and entity.
    /// </summary>
    private static void MapTraits(object source, GameEntity target, PropertyInfo sourceProperty)
    {
        var value = sourceProperty.GetValue(source);
        if (value == null) return;

        // Get TraitContainerComponent from entity
        var traitComponent = target.GetComponent<ITraitContainerComponent>();
        if (traitComponent == null)
        {
            throw new InvalidOperationException(
                $"Entity '{target.Name}' does not have a TraitContainerComponent. " +
                $"Cannot map traits from property '{sourceProperty.Name}'.");
        }

        // Verify property is an enumerable of IBaseTrait
        if (value is not IEnumerable<IBaseTrait> traits)
        {
            throw new InvalidOperationException(
                $"Property '{sourceProperty.Name}' marked with [MapToTraits] must be IEnumerable<IBaseTrait>. " +
                $"Found type: {sourceProperty.PropertyType.Name}");
        }

        // Copy each trait to the component
        foreach (var trait in traits)
        {
            // For now, we add the trait reference directly.
            // If traits need to be cloned, implement ICloneable on IBaseTrait
            traitComponent.AddTrait(trait);
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