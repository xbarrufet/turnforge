using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Entities.Appliers;

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
            [new AgentSpawnedResult(
                agent.Id,
                agent.DefinitionId,
                agent.PositionComponent.CurrentPosition)]);
    }
}
