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
using System.Reflection;

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
        EngineAutoMapper.Map(definition, prop);
        EngineAutoMapper.Map(definition, prop);
        EngineAutoMapper.Map(descriptor, prop);

        // Add extra components from descriptor
        foreach (var component in descriptor.ExtraComponents)
        {
            prop.AddComponent(component);
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
        EngineAutoMapper.Map(definition, agent);
        EngineAutoMapper.Map(definition, agent);
        EngineAutoMapper.Map(descriptor, agent);

        // Add extra components from descriptor
        foreach (var component in descriptor.ExtraComponents)
        {
            agent.AddComponent(component);
        }

        return agent;
    }

    /// Determines the concrete entity type from EntityType attribute on descriptor or definition
/// </summary>
private Type GetEntityType<TDefault>(Type descriptorType, BaseGameEntityDefinition definition) 
    where TDefault : GameEntity
{
    // Priority 1: Check descriptor type
    var entityType = descriptorType.GetCustomAttribute<EntityTypeAttribute>()?.EntityType;
    
    // Priority 2: Check definition type
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

