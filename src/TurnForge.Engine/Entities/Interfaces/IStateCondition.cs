namespace TurnForge.Engine.Entities.Interfaces;

public interface IStateCondition<TState>
{
    bool IsMet(TState state);
}