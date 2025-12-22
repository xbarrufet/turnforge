using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Decisions.Spawn;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Appliers.Interfaces;

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
        
        // 3. Create effect with metadata
        var effect = new EntitySpawnedEffect(
            entityId: agent.Id,
            definitionId: agent.DefinitionId,
            entityType: "Agent",
            category: agent.Category,
            position: agent.PositionComponent.CurrentPosition
        );
        
        return new ApplierResponse(newState, [effect]);
    }
}
