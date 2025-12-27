using TurnForge.Engine.Definitions;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Appliers.Entity.Interfaces;

/// <summary>
/// Applier for spawn decisions that carry pre-created entities.
/// Simply adds the entity to GameState.
/// </summary>
public interface ISpawnApplier<in TDecision, TEntity> : IApplier<TDecision>
    where TDecision : ISpawnDecision<TEntity>
    where TEntity : GameEntity
{
}
