using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Entities.Actors; // For Agent, Prop
using TurnForge.Engine.Entities.Board; // For ZoneDescriptors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces; // UPDATED
using TurnForge.Engine.Definitions.Factories.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Services;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers;

/// <summary>
/// Default implementation of IEntityApplier.
/// Creates entities using the GenericEntityFactory and returns SpawnEntityOperations.
/// </summary>
public sealed class EntityApplier : IEntityApplier
{
    private readonly IGameCatalog _catalog;
    private readonly TraitInitializationService _traitService;
    private readonly IGameEntityFactory _factory;
    
    public EntityApplier(IGameCatalog catalog, TraitInitializationService traitService, IGameEntityFactory factory)
    {
        _catalog = catalog;
        _traitService = traitService;
        _factory = factory;
    }
    
    /// <summary>
    /// Create entity from Definition + Position.
    /// Used for runtime spawns (Zombicide end-of-round, triggers).
    /// </summary>
    public SpawnEntityOperation Apply(BaseGameEntityDefinition definition, IBoardPosition position)
    {
        // Create a minimal descriptor from definition
        var descriptor = new GameEntityBuildDescriptor(definition.DefinitionId);
        
        // Copy traits from definition to descriptor's requested traits
        foreach (var trait in definition.Traits)
        {
            descriptor.RequestedTraits.Add(trait);
        }
        
        return ApplyInternal(descriptor, position);
    }
    
    /// <summary>
    /// Create entity from Descriptor + Position.
    /// Used for player deployment with loadout/overrides.
    /// </summary>
    public SpawnEntityOperation Apply(IGameEntityBuildDescriptor descriptor, IBoardPosition position)
    {
        return ApplyInternal(descriptor, position);
    }
    
    private SpawnEntityOperation ApplyInternal(IGameEntityBuildDescriptor descriptor, IBoardPosition position)
    {
        // Use factory to create entity based on descriptor type
        GameEntity entity = CreateEntity(descriptor);
        
        return new SpawnEntityOperation(entity.Id, entity, position);
    }
    
    private GameEntity CreateEntity(IGameEntityBuildDescriptor descriptor)
    {
        // Determine entity type from descriptor and create appropriately
        // This delegates to the factory which handles type resolution
        
        // For now, create a generic Agent - the factory will use EntityTypeRegistry
        // to determine the correct concrete type
        var definition = _catalog.GetDefinition<BaseGameEntityDefinition>(descriptor.DefinitionId);
        
        // Get identity trait for category
        var identity = definition.Traits
            .OfType<Traits.Standard.IdentityTrait>()
            .FirstOrDefault();
        var category = identity?.Category ?? "Entity";
        
        // Create entity with all components initialized
        var entity = new Agent(
            EntityId.New(),
            descriptor.DefinitionId,
            descriptor.DefinitionId, // name
            category
        );
        
        // Initialize traits from definition
        var traitContainer = entity.GetComponent<Components.Interfaces.ITraitContainerComponent>();
        if (traitContainer != null)
        {
            foreach (var trait in definition.Traits)
            {
                traitContainer.AddTrait(trait);
            }
            
            // Add requested override traits
            foreach (var trait in descriptor.RequestedTraits)
            {
                traitContainer.AddTrait(trait);
            }
        }
        
        // Initialize components from traits
        _traitService.InitializeComponents(entity);
        
        // Add extra components
        if (descriptor.ExtraComponents != null)
        {
            foreach (var component in descriptor.ExtraComponents)
            {
                entity.AddComponent((dynamic)component);
            }
        }
        
        return entity;
    }
}
