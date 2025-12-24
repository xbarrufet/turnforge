using System.Reflection;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Core.Registries;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Factories;

/// <summary>
/// Helper to build descriptors from SpawnRequests and Definitions.
/// Applies definition data and property overrides automatically.
/// This is used by CommandHandlers to preprocess SpawnRequests into fully populated Descriptors.
/// </summary>
public static class DescriptorBuilder
{
    /// <summary>
    /// Creates a descriptor from a SpawnRequest, applying definition and overrides.
    /// </summary>
    /// <typeparam name="TDescriptor">Type of descriptor to create</typeparam>
    /// <param name="request">Spawn request with definition ID and overrides</param>
    /// <param name="definition">Entity definition to map from</param>
    /// <returns>Fully populated descriptor ready for strategy processing</returns>
    /// <remarks>
    /// Process:
    /// 1. Create descriptor instance (respects [DescriptorType] attribute if present)
    /// 2. Apply property overrides from request
    /// 3. Apply position if specified in request
    /// 
    /// Note: We don't use EngineAutoMapper here because descriptors don't have components yet.
    /// The descriptor properties will be mapped to entity components during entity creation in GenericActorFactory.
    /// </remarks>
    public static TDescriptor Build<TDescriptor>(
        SpawnRequest request,
        BaseGameEntityDefinition definition)
        where TDescriptor : IGameEntityBuildDescriptor
    {
        // 1. Create descriptor instance
        var descriptor = CreateDescriptor<TDescriptor>(request.DefinitionId, definition);
        
        // 2. Apply property overrides if present
        if (request.PropertyOverrides != null && request.PropertyOverrides.Count > 0)
        {
            ApplyOverrides(descriptor, request.PropertyOverrides);
        }
        
        // 3. Apply position if specified (not Position.Empty)
        if (request.Position != Position.Empty)
        {
            SetPosition(descriptor, request.Position);
        }

        // 4. Copy extra components if present
        if (request.ExtraComponents != null)
        {
            descriptor.ExtraComponents.AddRange(request.ExtraComponents);
        }
        
        return descriptor;
    }
    
    /// <summary>
    /// Creates descriptor instance, respecting descriptor type hierarchy.
    /// </summary>
    /// <remarks>
    /// Priority:
    /// 1. Entity's [DescriptorType] attribute (via EntityTypeRegistry)
    /// 2. Definition's [DescriptorType] attribute (legacy support)
    /// 3. Default TDescriptor type
    /// 
    /// Lookup chain: Definition → Entity → Descriptor
    /// </remarks>
    private static TDescriptor CreateDescriptor<TDescriptor>(
        string definitionId,
        BaseGameEntityDefinition definition)
        where TDescriptor : IGameEntityBuildDescriptor
    {
        Type descriptorType;
        
        // 1. Try to get descriptor type via Definition → Entity → Descriptor chain
        var entityType = EntityTypeRegistry.GetEntityType(definition.GetType());
        
        if (entityType != null)
        {
            // Get descriptor type from entity's [DescriptorType] attribute
            var descriptorTypeFromEntity = EntityTypeRegistry.GetDescriptorType(entityType);
            
            if (descriptorTypeFromEntity != null)
            {
                descriptorType = descriptorTypeFromEntity;
                
                // Validate compatibility
                if (!typeof(TDescriptor).IsAssignableFrom(descriptorType))
                {
                    throw new InvalidOperationException(
                        $"DescriptorType '{descriptorType.Name}' on entity '{entityType.Name}' " +
                        $"is not compatible with expected type '{typeof(TDescriptor).Name}'");
                }
                
                // Create instance and return
                return (TDescriptor)Activator.CreateInstance(descriptorType, definitionId)!;
            }
        }
        
        // 2. Legacy: Check for [DescriptorType] attribute on definition (backwards compatibility)
        var descriptorTypeAttr = definition.GetType()
            .GetCustomAttribute<DescriptorTypeAttribute>();
        
        if (descriptorTypeAttr != null)
        {
            descriptorType = descriptorTypeAttr.DescriptorType;
            
            // Validate it's compatible with TDescriptor
            if (!typeof(TDescriptor).IsAssignableFrom(descriptorType))
            {
                throw new InvalidOperationException(
                    $"DescriptorType '{descriptorType.Name}' on definition '{definition.DefinitionId}' " +
                    $"is not compatible with expected type '{typeof(TDescriptor).Name}'");
            }
            
            return (TDescriptor)Activator.CreateInstance(descriptorType, definitionId)!;
        }
        
        // 3. Use default TDescriptor
        descriptorType = typeof(TDescriptor);
        
        // Create instance via reflection (constructor with string parameter)
        var descriptor = (TDescriptor)Activator.CreateInstance(descriptorType, definitionId)!;
        
        return descriptor;
    }
    
    /// <summary>
    /// Applies property overrides to descriptor using reflection.
    /// </summary>
    private static void ApplyOverrides<TDescriptor>(
        TDescriptor descriptor,
        Dictionary<string, object> overrides)
        where TDescriptor : IGameEntityBuildDescriptor
    {
        var descriptorType = descriptor.GetType();
        
        foreach (var (propertyName, value) in overrides)
        {
            var prop = descriptorType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            
            if (prop == null)
            {
                // Property not found - log warning and continue
                // (Could throw exception if strict validation is needed)
                continue;
            }
            
            if (!prop.CanWrite)
            {
                // Property is readonly - skip
                continue;
            }
            
            try
            {
                // Try to set the value (may require type conversion)
                prop.SetValue(descriptor, value);
            }
            catch (ArgumentException)
            {
                // Type mismatch - log warning and continue
                // (Could throw exception if strict validation is needed)
            }
        }
    }
    
    /// <summary>
    /// Sets position on descriptor if it has a Position property.
    /// </summary>
    private static void SetPosition<TDescriptor>(TDescriptor descriptor, Position position)
        where TDescriptor : IGameEntityBuildDescriptor
    {
        // Try to find Position property via reflection
        var positionProp = descriptor.GetType()
            .GetProperty("Position", BindingFlags.Public | BindingFlags.Instance);
        
        if (positionProp != null && positionProp.CanWrite && positionProp.PropertyType == typeof(Position))
        {
            positionProp.SetValue(descriptor, position);
        }
    }
}
