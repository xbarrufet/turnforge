using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class AgentApplier(IGameEntityFactory<Agent> factory) : IBuildApplier<AgentSpawnDecision, Agent>
{
    public ApplierResponse Apply(AgentSpawnDecision decision, GameState state)
    {
        var agent = factory.Build(decision.Descriptor);
        return new ApplierResponse(state.WithAgent(agent), [new AgentSpawnedResult(agent.Id, agent.Definition.TypeId, decision.Position)]);
    }
}
