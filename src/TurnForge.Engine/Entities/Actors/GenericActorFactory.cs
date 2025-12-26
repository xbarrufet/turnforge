using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Mapping;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Core.Registries;
using System.Reflection;
using TurnForge.Engine.Values;

namespace TurnForge.Engine.Entities.Actors;

public sealed class GenericActorFactory(
    IGameCatalog gameCatalog)
    : IActorFactory
{
    public Prop BuildProp(PropDescriptor descriptor)
    {
        var definition = gameCatalog.GetDefinition<BaseGameEntityDefinition>(descriptor.DefinitionID);
        
        // Determine concrete type from attributes
        var entityType = GetEntityType<Prop>(descriptor.GetType(), definition);
        
        // Create instance using reflection
        var prop = CreateEntityInstance<Prop>(entityType, descriptor.DefinitionID, definition);

        // Map properties from definition and descriptor to components
        PropertyAutoMapper.Map(definition, prop);
        PropertyAutoMapper.Map(definition, prop);
        PropertyAutoMapper.Map(descriptor, prop);

        // Direct position assignment (simple and explicit)
        if (descriptor.Position != Position.Empty)
        {
            prop.PositionComponent.CurrentPosition = descriptor.Position;
        }

        // Add extra components from descriptor
        foreach (var component in descriptor.ExtraComponents)
        {
            prop.AddComponent((dynamic)component);
        }

        // Process dynamic attributes from definition
        if (definition.Attributes.Count > 0)
        {
            var attributeValues = new Dictionary<string, AttributeValue>();
            foreach (var kvp in definition.Attributes)
            {
                if (kvp.Value is int intVal)
                {
                    attributeValues[kvp.Key] = new AttributeValue(intVal);
                }
                else if (kvp.Value is string strVal)
                {
                    var dice = DiceThrowType.Parse(strVal);
                    attributeValues[kvp.Key] = new AttributeValue(dice);
                }
                // Ignore other types or log warning
            }
            
            if (attributeValues.Count > 0)
            {
                 prop.AddComponent(new AttributeComponent(attributeValues));
            }
        }

        return prop;
    }

     public Agent BuildAgent(AgentDescriptor descriptor)
{
    var definition = gameCatalog.GetDefinition<BaseGameEntityDefinition>(descriptor.DefinitionID);
    
    // Determine concrete type from attributes
    var entityType = GetEntityType<Agent>(descriptor.GetType(), definition);
    
    // Create instance using reflection
    var agent = CreateEntityInstance<Agent>(entityType, descriptor.DefinitionID, definition);

    // Map properties from definition and descriptor to components
    PropertyAutoMapper.Map(definition, agent);
    PropertyAutoMapper.Map(descriptor, agent);

    // Direct position assignment (simple and explicit)
    if (descriptor.Position != Position.Empty)
    {
        agent.PositionComponent.CurrentPosition = descriptor.Position;
    }
    
    // Team: descriptor overrides definition
    agent.Team = descriptor.Team ?? definition.Team ?? string.Empty;
    
    // Controller: descriptor overrides definition
    agent.ControllerId = descriptor.ControllerId ?? definition.ControllerId;

    // Add extra components from descriptor
    foreach (var component in descriptor.ExtraComponents)
    {
        // Use dynamic to dispatch to AddComponent<T> with the runtime type of the component
        // This ensures the component is registered under its concrete type (or specific interface if casted)
        // rather than IGameEntityComponent, allowing GetComponent lookups to work via IsAssignableFrom.
        agent.AddComponent((dynamic)component);
    }
    
    // Process dynamic attributes from definition
    if (definition.Attributes.Count > 0)
    {
        var attributeValues = new Dictionary<string, AttributeValue>();
        foreach (var kvp in definition.Attributes)
        {
            if (kvp.Value is int intVal)
            {
                attributeValues[kvp.Key] = new AttributeValue(intVal);
            }
            else if (kvp.Value is string strVal)
            {
                var dice = DiceThrowType.Parse(strVal);
                attributeValues[kvp.Key] = new AttributeValue(dice);
            }
            // Ignore other types or log warning
        }
        
        if (attributeValues.Count > 0)
        {
             agent.AddComponent(new AttributeComponent(attributeValues));
        }
    }

    return agent;
}

    /// <summary>
    /// Determines the concrete entity type using EntityTypeRegistry.
    /// </summary>
    /// <remarks>
    /// Lookup chain:
    /// 1. Definition → EntityTypeRegistry → Entity type
    /// 2. Definition's [EntityType] attribute (legacy)
    /// 3. Default TDefault type
    /// </remarks>
    private Type GetEntityType<TDefault>(Type descriptorType, BaseGameEntityDefinition definition) 
        where TDefault : GameEntity
    {
        // Priority 1: Use registry (Definition → Entity)
        var entityType = EntityTypeRegistry.GetEntityType(definition.GetType());
        
        // Priority 2: Check definition's [EntityType] attribute (legacy support)
        if (entityType == null)
        {
            entityType = definition.GetType().GetCustomAttribute<EntityTypeAttribute>()?.EntityType;
        }
        
        // Priority 3: Use default type
        return entityType ?? typeof(TDefault);
    }

/// <summary>
/// Creates entity instance using reflection
/// </summary>
private T CreateEntityInstance<T>(Type concreteType, string definitionId, BaseGameEntityDefinition definition) 
    where T : GameEntity
{
    var instance = Activator.CreateInstance(
        concreteType, 
        EntityId.New(), 
        definitionId, 
        definition.Name, 
        definition.Category);
    
    if (instance == null)
    {
        throw new InvalidOperationException($"Failed to create instance of {concreteType.Name}");
    }
    
    return (T)instance;
}

}

