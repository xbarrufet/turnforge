using TurnForge.Engine.Definitions;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Appliers.Entity.Interfaces;

public interface IBuildApplier<in TDecision, TEntity> : IApplier<TDecision>
    where TDecision : IBuildDecision<TEntity>
    where TEntity : GameEntity
{
}