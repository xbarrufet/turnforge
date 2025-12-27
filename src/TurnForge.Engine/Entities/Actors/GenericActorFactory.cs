using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Definitions.Descriptors.Interfaces;
using TurnForge.Engine.Definitions.Factories.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Core.Registries;
using System.Reflection;
using TurnForge.Engine.Values;
using TurnForge.Engine.Services;

namespace TurnForge.Engine.Definitions.Actors;

public sealed class GenericActorFactory(
    IGameCatalog gameCatalog,TraitInitializationService traitService)
    : IActorFactory
{

    public Prop BuildProp(PropDescriptor descriptor)
    {
        var definition = gameCatalog.GetDefinition<BaseGameEntityDefinition>(descriptor.DefinitionId);
        
        // Determine concrete type from attributes
        var entityType = GetEntityType<Prop>(descriptor.GetType(), definition);
        
        // Create instance using reflection
        var prop = CreateEntityInstance<Prop>(entityType, descriptor.DefinitionId, definition);

        // Position: Handled via TraitInitializationService if PositionTrait is present
        // But Descriptor.Position is gone.
        // If a PositionTrait was passed in RequestedTraits, it will be added to container.
        // We rely on TraitInitializationService to check for PositionTrait and update PositionComponent.
        // OR we manually check for PositionTrait here if TraitService doesn't do it yet for Position.
        // Given Phase 3 refactor of PositionTrait, let's assume TraitService handles it or
        // we explicitly check here for robustness as Position is critical.
        
        // Initialize Traits/Components
        InitializeTraits(prop, definition, descriptor.RequestedTraits);
        traitService.InitializeComponents(prop);    

        return prop;
    }

    public Agent BuildAgent(AgentDescriptor descriptor)
    {
        var definition = gameCatalog.GetDefinition<BaseGameEntityDefinition>(descriptor.DefinitionId);
        
        // Determine concrete type from attributes
        var entityType = GetEntityType<Agent>(descriptor.GetType(), definition);
        
        // Create instance using reflection
        var agent = CreateEntityInstance<Agent>(entityType, descriptor.DefinitionId, definition);
    
        // Initialize Traits (Definition + Overrides)
        InitializeTraits(agent, definition, descriptor.RequestedTraits);
        
        // Initialize Components from Traits
        traitService.InitializeComponents(agent);
        
        // Add extra components from descriptor if any (legacy or manual)
        if (descriptor.ExtraComponents != null)
        {
            foreach (var component in descriptor.ExtraComponents)
            {
                // Use dynamic to dispatch to AddComponent<T> with the runtime type of the component
                agent.AddComponent((dynamic)component);
            }
        }
        
        return agent;
    }

    private void InitializeTraits(GameEntity entity, BaseGameEntityDefinition definition, IEnumerable<TurnForge.Engine.Traits.Interfaces.IBaseTrait> requestedTraits)
    {
        var traitContainer = entity.GetComponent<ITraitContainerComponent>();
        if (traitContainer != null)
        {
            // 1. Add Definition Traits
            foreach (var trait in definition.Traits)
            {
                traitContainer.AddTrait(trait);
            }

            // 2. Add Requested Override Traits
            if (requestedTraits != null)
            {
                foreach (var trait in requestedTraits)
                {
                    traitContainer.AddTrait(trait);
                }
            }
        }
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
        
        // Priority 3: Use default type
        return entityType ?? typeof(TDefault);
    }

/// <summary>
/// Creates entity instance using reflection
/// </summary>
private T CreateEntityInstance<T>(Type concreteType, string definitionId, BaseGameEntityDefinition definition) 
    where T : GameEntity
{
    // Extract Identity from Trait
    var identity = definition.Traits.OfType<TurnForge.Engine.Traits.Standard.IdentityTrait>().FirstOrDefault();
    var category = identity?.Category ?? "Common";
    var name = definitionId; // Use definitionId as name if no specific name

    var instance = Activator.CreateInstance(
        concreteType, 
        EntityId.New(), 
        definitionId, 
        name,
        category);
    
    if (instance == null)
    {
        throw new InvalidOperationException($"Failed to create instance of {concreteType.Name}");
    }
    
    return (T)instance;
}

}

