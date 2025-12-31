namespace TurnForge.Engine.Entities.Interfaces;

public interface IStateRule<in TState, out TResult>
{
    TResult Resolve(TState state);
}