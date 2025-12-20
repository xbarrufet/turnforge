using BarelyAlive.Rules.Events;
using BarelyAlive.Rules.Infrastructure;
using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Events;



public sealed class TurnForgeEventHandler(IBarelyAliveEffectsSink effectsSink) : ITurnForgeEffectsHandler
{


    public IGameEffect? LastEffect { get; private set; }

    public IBarelyAliveEffectsSink EffectsSink => effectsSink;

    public void Handle(IGameEffect effect)
    {
        LastEffect = effect;
        switch (effect)
        {
            case AgentSpawnedEffect a:
                EffectsSink.Emit(new AgentSpawned(a.AgentId.ToString(), a.AgentType.ToString(), a.Position.ToString()));
                break;
            case PropSpawnedEffect p:
                EffectsSink.Emit(new PropSpawned(p.PropId.ToString(), p.PropType.ToString(), p.Position.ToString()));
                break;
            default:
                break;
        }
    }
}