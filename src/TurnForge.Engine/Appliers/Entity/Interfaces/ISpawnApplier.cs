using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

/// <summary>
/// Applier for spawn decisions that carry pre-created entities.
/// Simply adds the entity to GameState.
/// </summary>
public interface ISpawnApplier<in TDecision, TEntity> : IApplier<TDecision>
    where TDecision : ISpawnDecision<TEntity>
    where TEntity : GameEntity
{
}
