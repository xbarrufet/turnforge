using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IApplier<T> where T : class, IGameEntityComponent
{
    T Apply(T component);
}