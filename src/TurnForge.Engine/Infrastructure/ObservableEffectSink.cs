using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class ObservableEffectSink : IEffectSink
{
    private readonly List<Action<IGameEffect>> _subscribers = new();

    public void Emit(IGameEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        foreach (var subscriber in _subscribers)
            subscriber(effect);
    }

    public void Subscribe(Action<IGameEffect> handler)
    {
        _subscribers.Add(handler);
    }
}