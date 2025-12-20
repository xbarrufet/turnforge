using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;

namespace TurnForge.Engine.Orchestrator.Interfaces;

public interface IOrchestrator
{
    GameState CurrentState { get; }
    void SetState(GameState state);
    void RegisterApplier<TDecision>(IApplier<TDecision> applier) where TDecision : IDecision;
    void RegisterFactory<TDescriptor, TEntity>(IGameEntityFactory<TEntity> factory)
        where TDescriptor : IGameEntityDescriptor<TEntity>
        where TEntity : GameEntity;
    void Enqueue(IEnumerable<IDecision> decisions);
    void ExecuteScheduled(string? phase, string when);
}
