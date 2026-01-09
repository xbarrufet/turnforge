using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Core.Registries;
using System.Reflection;
using TurnForge.Engine.Values;
using TurnForge.Engine.Services;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board; // For Zone
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;

namespace TurnForge.Engine.Infrastructure.Factories;

public sealed class GenericEntityFactory(
    IGameCatalog gameCatalog, TraitInitializationService traitService)
    : IGameEntityFactory
{
    public Prop BuildProp(PropDescriptor descriptor) => BuildEntity<Prop>(descriptor);
    
    public Agent BuildAgent(AgentDescriptor descriptor) => BuildEntity<Agent>(descriptor);

    public Zone BuildZone(ZoneDescriptor descriptor) => BuildEntity<Zone>(descriptor);

    private T BuildEntity<T>(IGameEntityBuildDescriptor descriptor) where T : GameEntity
    {
        var definition = gameCatalog.GetDefinition<BaseGameEntityDefinition>(descriptor.DefinitionId);
        
        // Determine concrete type from attributes or registry
        var entityType = GetEntityType<T>(descriptor.GetType(), definition);
        
        // Create instance using reflection or specialized logic
        var entity = CreateEntityInstance<T>(entityType, descriptor, definition);
    
        // Initialize Traits (Definition + Overrides)
        InitializeTraits(entity, definition, descriptor.RequestedTraits);
        
        // Initialize Components from Traits
        traitService.InitializeComponents(entity);
        
        // Add extra components from descriptor if any (legacy or manual)
        if (descriptor.ExtraComponents != null)
        {
            foreach (var component in descriptor.ExtraComponents)
            {
                // Use dynamic to dispatch to AddComponent<T> with the runtime type of the component
                entity.AddComponent((dynamic)component);
            }
        }
        
        return entity;
    }

    private void InitializeTraits(GameEntity entity, BaseGameEntityDefinition definition, IEnumerable<TurnForge.Engine.Traits.Interfaces.IDataTrait> requestedTraits)
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
    private Type GetEntityType<TDefault>(Type descriptorType, BaseGameEntityDefinition definition) 
        where TDefault : GameEntity
    {
        // Priority 1: Use registry (Definition → Entity)
        var entityType = EntityTypeRegistry.GetEntityType(definition.GetType());
        
        // Priority 2: Use default type
        return entityType ?? typeof(TDefault);
    }

    private T CreateEntityInstance<T>(Type concreteType, IGameEntityBuildDescriptor descriptor, BaseGameEntityDefinition definition) 
        where T : GameEntity
    {
        // Extract Identity from Trait
        var identity = definition.Traits.OfType<TurnForge.Engine.Traits.Standard.IdentityTrait>().FirstOrDefault();
        var category = identity?.Category ?? "Common";
        var name = descriptor.DefinitionId; // Use definitionId as name if no specific name

        // Standard creation (Agent, Prop, Zone)
        var instance = Activator.CreateInstance(
            concreteType, 
            EntityId.New(), 
            descriptor.DefinitionId, 
            name,
            category);
        
        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to create instance of {concreteType.Name}");
        }
        
        return (T)instance;
    }
}

