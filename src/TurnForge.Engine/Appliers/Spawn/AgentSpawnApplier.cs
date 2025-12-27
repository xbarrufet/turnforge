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
/// Applier that creates Agent entities from spawn decisions.
/// Uses GenericActorFactory to build the agent from the descriptor.
/// Implements IApplier to integrate with Orchestrator/FSM.
/// </summary>
public sealed class AgentSpawnApplier : IApplier<SpawnDecision<AgentDescriptor>>
{
    private readonly GenericActorFactory _factory;
    
    public AgentSpawnApplier(GenericActorFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
    
    /// <summary>
    /// Apply the spawn decision: create agent and add to game state.
    /// </summary>
    public ApplierResponse Apply(SpawnDecision<AgentDescriptor> decision, GameState state)
    {
        // 1. Create agent from descriptor using factory
        var agent = _factory.BuildAgent(decision.Descriptor);
        
        // 2. Add agent to state (immutable update)
        var newState = state.WithAgent(agent);
        
        // 3. Create event with metadata
        var gameEvent = new EntitySpawnedEvent(
            entityId: agent.Id,
            definitionId: agent.DefinitionId,
            entityType: "Agent",
            category: agent.Category,
            position: agent.PositionComponent.CurrentPosition
        );
        
        return new ApplierResponse(newState, [gameEvent]);
    }
}
