

namespace TurnForge.Engine.Entities.Interfaces;

public interface IResolver<in TContext, out TResult>
{
    TResult Resolve(TContext context);
}