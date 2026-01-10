using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Entities.Actors; // For Agent, Prop
using TurnForge.Engine.Entities.Board; // For ZoneDescriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Descriptors;
// UPDATED
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Services;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Spawn;

/// <summary>
/// Default implementation of ISpawnService.
/// Creates entities using the GenericEntityFactory and returns SpawnEntityOperations.
/// </summary>
public sealed class SpawnService : ISpawnService
{
    private readonly IGameCatalog _catalog;
    private readonly ComponentInitializationService _traitService;
    private readonly IGameEntityFactory _factory;

    public SpawnService(IGameCatalog catalog, ComponentInitializationService traitService, IGameEntityFactory factory)
    {
        _catalog = catalog;
        _traitService = traitService;
        _factory = factory;
    }

    /// <summary>
    /// Spawn entity from Definition + Position.
    /// Used for runtime spawns (Zombicide end-of-round, triggers).
    /// </summary>
    public SpawnEntityOperation Spawn(BaseGameEntityDefinition definition, IBoardPositionId position)
    {
        // Create a minimal descriptor from definition
        var descriptor = new GameEntityBuildDescriptor(definition.DefinitionId,definition.DefinitionId);

        // Copy traits from definition to descriptor's trait values
        foreach (var trait in definition.Traits)
        {
            descriptor.DefinitionTraitValues.Add(trait);
        }

        return SpawnInternal(descriptor, position);
    }

    /// <summary>
    /// Spawn entity from Descriptor + Position.
    /// Used for player deployment with loadout/overrides.
    /// </summary>
    public SpawnEntityOperation Spawn(IGameEntityBuildDescriptor descriptor, IBoardPositionId position)
    {
        return SpawnInternal(descriptor, position);
    }

    public Actor PositionActor(Actor actor, IBoardPositionId position)
    {
        // Create a new Actor instance with updated position
        var positionedActor = actor.CloneWithNewPosition(position);
        return positionedActor;
    }

    private SpawnEntityOperation SpawnInternal(IGameEntityBuildDescriptor descriptor, IBoardPositionId position)
    {
        // Use factory to create entity based on descriptor type
        //GameEntity entity = CreateEntity(descriptor);
        
        //return new SpawnEntityOperation(entity, position); TODO
        return new SpawnEntityOperation();
    }

    
   
    
    
}
