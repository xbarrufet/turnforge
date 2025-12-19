using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class ObservableEffectSink : IEffectSink
{
    private readonly List<Action<IGameEffect>> _subscribers = new();

    public void Emit(IGameEffect effect)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));

        foreach (var subscriber in _subscribers)
            subscriber(effect);
    }

    public void Subscribe(Action<IGameEffect> handler)
    {
        _subscribers.Add(handler);
    }
}