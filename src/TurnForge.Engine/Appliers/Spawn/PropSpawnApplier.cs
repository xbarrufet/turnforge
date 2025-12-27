using TurnForge.Engine.Core;
using TurnForge.Engine.Events;
using TurnForge.Engine.Decisions.Spawn;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Definitions.Actors.Interfaces;

namespace TurnForge.Engine.Appliers.Spawn;

/// <summary>
/// Applier that creates Prop entities from spawn decisions.
/// Uses GenericActorFactory to build the prop from the descriptor.
/// Implements IApplier to integrate with Orchestrator/FSM.
/// </summary>
public sealed class PropSpawnApplier : IApplier<SpawnDecision<PropDescriptor>>
{
    private readonly GenericActorFactory _factory;
    
    public PropSpawnApplier(GenericActorFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
    
    /// <summary>
    /// Apply the spawn decision: create prop and add to game state.
    /// </summary>
    public ApplierResponse Apply(SpawnDecision<PropDescriptor> decision, GameState state)
    {
        // 1. Create prop from descriptor using factory
        var prop = _factory.BuildProp(decision.Descriptor);
        
        // 2. Add prop to state (immutable update)
        var newState = state.WithProp(prop);
        
        // 3. Create event with metadata
        var gameEvent = new EntitySpawnedEvent(
            entityId: prop.Id,
            definitionId: prop.DefinitionId,
            entityType: "Prop",
            category: prop.Category,
            position: prop.PositionComponent.CurrentPosition
        );
        
        return new ApplierResponse(newState, [gameEvent]);
    }
}
