using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure.Appliers.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure.Appliers;

public sealed class SpawnApplier(IActorFactory actorFactory,IEffectSink effectsSink) : ISpawnApplier
{
    private readonly IActorFactory _actorFactory = actorFactory;
    private readonly IEffectSink _effectsSink = effectsSink;
    
    public GameState Apply(IEnumerable<ISpawnDecision> decisions, GameState state)
    {
        foreach (var decision in decisions)
        {
            var result = decision switch
            {
                UnitSpawnDecision u => SpawnUnit(u, state),
                PropSpawnDecision p => SpawnProp(p, state),
                _ => throw new InvalidOperationException(
                    $"Unknown spawn decision type: {decision.GetType().Name}")
            };
            _effectsSink.Emit(result.GameEffect);
            state = result.GameState;
        }
        return state;
    }
    
    private IAppplierResult SpawnUnit(UnitSpawnDecision decision, GameState currentState)
    {
        var unit = _actorFactory.BuildUnit(
            decision.TypeId,
            decision.Position,
            decision.ExtraBehaviours);
        return new ApplierResult(currentState.WithUnit(unit), new UnitSpawnedEffect(unit.Id, decision.Position));
    }
    
    private IAppplierResult SpawnProp(PropSpawnDecision decision, GameState currentState)
    {
        var prop = _actorFactory.BuildProp(
            decision.TypeId,
            decision.Position,
            decision.ExtraBehaviours);
        return new ApplierResult(currentState.WithProp(prop), new PropSpawnedEffect(prop.Id, decision.Position));
    }
}