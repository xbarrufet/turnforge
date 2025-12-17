using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class ObservableEffectSink : IEffectSink
{
    private readonly List<IGameEffect> _effects = new();

    public IReadOnlyList<IGameEffect> Effects => _effects;

    public void Emit(IGameEffect effect)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));

        _effects.Add(effect);
    }
}