using TurnForge.Engine.Core.Interfaces;

public interface ITurnForgeEffectsHandler
{
    IGameEffect? LastEffect { get; }
    void Handle(IGameEffect effect);
}