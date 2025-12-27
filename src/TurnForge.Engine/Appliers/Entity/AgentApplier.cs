using TurnForge.Engine.Events;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Appliers.Entity;

/// <summary>
/// Applier that adds a pre-created Agent entity to the GameState.
/// The Strategy is responsible for creating the entity via Factory.
/// </summary>
public sealed class AgentApplier : ISpawnApplier<AgentSpawnDecision, Agent>
{
    public ApplierResponse Apply(AgentSpawnDecision decision, GameState state)
    {
        var agent = decision.Entity;
        
        return new ApplierResponse(
            state.WithAgent(agent), 
            [new AgentSpawnedEvent(
                agent.Id,
                agent.DefinitionId,
                agent.PositionComponent.CurrentPosition)]);
    }
}
