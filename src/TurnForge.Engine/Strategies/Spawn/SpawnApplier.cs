using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.States;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Strategies.Spawn;

internal sealed class SpawnApplier(GameState game, IActorFactory actorFactory,IEffectSink effects) : ISpawnApplier
{
    private readonly GameState _gameState = game;
    private readonly IActorFactory _actorFactory = actorFactory;
    private readonly IEffectSink _effects = effects;


    public void Spawn(PropSpawnDecision decision)
    {
        var prop = _actorFactory.BuildProp(decision.Descriptor,decision.Position);
        _gameState.AddActor(prop);
        _effects.Emit(new PropSpawnedEffect(prop.Id, prop.Position));
    }

    public void Spawn(UnitSpawnDecision decision)
    {
        var unit= _actorFactory.BuildUnit(decision.Descriptor,decision.Position);
        _gameState.AddActor(unit);
        _effects.Emit(new UnitSpawnedEffect(unit.Id, unit.Position));
    }

    public void Spawn(HostileSpawnDecision decision)
    {
        var hostile = _actorFactory.BuildHostile(decision.Descriptor,decision.Position);
        _gameState.AddActor(hostile);
        _effects.Emit(new HostileSpawnedEffect(hostile.Id, hostile.Position));
        
    }
}