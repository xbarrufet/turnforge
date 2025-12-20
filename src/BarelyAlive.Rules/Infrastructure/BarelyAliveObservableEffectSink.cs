using BarelyAlive.Rules.Events.Interfaces;

namespace BarelyAlive.Rules.Infrastructure;

public sealed class BarelyAliveObservableEffectSink : IBarelyAliveEffectsSink
{
    private readonly List<Action<IBarelyAliveEffect>> _subscribers = new();

    public void Emit(IBarelyAliveEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        for (int i = 0; i < _subscribers.Count; i++)
        {
            Action<IBarelyAliveEffect>? subscriber = _subscribers[i];
            subscriber(effect);
        }
    }

    public void Subscribe(Action<IBarelyAliveEffect> handler)
    {
        _subscribers.Add(handler);
    }
}