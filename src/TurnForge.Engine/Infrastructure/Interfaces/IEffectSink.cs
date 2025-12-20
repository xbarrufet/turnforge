namespace TurnForge.Engine.Core.Interfaces;

public interface IEffectSink
{
    void Emit(IGameEffect effect);
    void Subscribe(Action<IGameEffect> handler);
}