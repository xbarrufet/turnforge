using TurnForge.Engine.Entities;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Decisions.Entity.Interfaces;

/// <summary>
/// Spawn decision that carries a pre-created entity ready to be added to GameState.
/// Strategies are responsible for creating entities via Factory.
/// </summary>
/// <typeparam name="T">The entity type being spawned</typeparam>
public interface ISpawnDecision<T> : IDecision where T : GameEntity
{
    /// <summary>
    /// The already-created entity ready to be added to GameState
    /// </summary>
    T Entity { get; }
}