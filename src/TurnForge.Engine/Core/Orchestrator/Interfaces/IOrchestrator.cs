using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Core.Interfaces; 

namespace TurnForge.Engine.Core.Orchestrator.Interfaces;

public interface IOrchestrator
{
    GameState CurrentState { get; }
    void SetState(GameState state);
    void RegisterApplier<TDecision>(IApplier<TDecision> applier) where TDecision : IDecision;
    void RegisterFactory<TDescriptor, TEntity>(IGameEntityFactory<TEntity> factory)
        where TDescriptor : IGameEntityDescriptor<TEntity>
        where TEntity : GameEntity;
    IGameEvent[] ExecuteScheduled(string? phase, string when);
    IGameEvent[] Apply(IDecision decision);
    void Enqueue(IEnumerable<IDecision> decisions);
    void SetLogger(IGameLogger logger);
}
