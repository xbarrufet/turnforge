using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Events;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class SpawnApplier(IActorFactory actorFactory, IEffectSink effectsSink) : ISpawnApplier
{
    private readonly IActorFactory _actorFactory = actorFactory;
    private readonly IEffectSink _effectsSink = effectsSink;

    public GameState Apply(IEnumerable<ISpawnDecision> decisions, GameState state)
    {
        foreach (var decision in decisions)
        {
            var result = decision switch
            {
                AgentSpawnDecision u => SpawnAgent(u, state),
                PropSpawnDecision p => SpawnProp(p, state),
                _ => throw new InvalidOperationException(
                    $"Unknown spawn decision type: {decision.GetType().Name}")
            };
            _effectsSink.Emit(result.GameEffect);
            state = result.GameState;
        }
        return state;
    }

    private IApplierResult SpawnAgent(AgentSpawnDecision decision, GameState currentState)
    {
        var descriptor = new AgentDescriptor(
            decision.TypeId,
            decision.Position,
            decision.ExtraBehaviours);

        var agent = _actorFactory.BuildAgent(descriptor);

        return new ApplierResult(currentState.WithAgent(agent), new AgentSpawnedEffect(agent.Id, agent.Definition.TypeId, decision.Position));
    }

    private IApplierResult SpawnProp(PropSpawnDecision decision, GameState currentState)
    {
        var descriptor = new PropDescriptor(
            decision.TypeId,
            decision.Position,
            decision.ExtraBehaviours);

        var prop = _actorFactory.BuildProp(descriptor);

        return new ApplierResult(currentState.WithProp(prop), new PropSpawnedEffect(prop.Id, prop.Definition.TypeId, decision.Position));
    }
}