using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IBuildApplier<in TDecision, TEntity> : IApplier<TDecision>
    where TDecision : IBuildDecision<TEntity>
    where TEntity : GameEntity
{
}