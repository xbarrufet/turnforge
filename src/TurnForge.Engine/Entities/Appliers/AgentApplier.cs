using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Events;
using TurnForge.Engine.Strategies.Spawn;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class AgentApplier(IGameEntityFactory<Agent> factory, IEffectSink effectsSink) : IBuildApplier<AgentSpawnDecision, Agent>
{
    public GameState Apply(AgentSpawnDecision decision, GameState state)
    {
        var agent = factory.Build(decision.Descriptor);
        effectsSink.Emit(new AgentSpawnedEffect(agent.Id, agent.Definition.TypeId, decision.Position));
        return state.WithAgent(agent);
    }
}
