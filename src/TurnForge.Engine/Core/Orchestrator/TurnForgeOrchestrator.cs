using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Core.Orchestrator.Interfaces;
using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core.Orchestrator;

public sealed class TurnForgeOrchestrator : IOrchestrator
{
    private readonly Dictionary<Type, object> _appliers = new();
    private readonly Dictionary<Type, object> _factories = new();

    public GameState CurrentState { get; private set; } = GameState.Empty();

    public void SetState(GameState state)
    {
        CurrentState = state;
    }

    public void RegisterApplier<TDecision>(IApplier<TDecision> applier) where TDecision : IDecision
    {
        _appliers[typeof(TDecision)] = applier;
    }

    public void RegisterFactory<TDescriptor, TEntity>(IGameEntityFactory<TEntity> factory)
        where TDescriptor : IGameEntityDescriptor<TEntity>
        where TEntity : GameEntity
    {
        _factories[typeof(TEntity)] = factory; // Mapping by Entity Type usually
        // Spec says "Factory Registry... Descriptor?". IOrchestrator signature has TDescriptor.
        // But Applier logic usually asks "Give me factory for Entity T".
        // I will store by TEntity for now.
    }

    public void Enqueue(IEnumerable<IDecision> decisions)
    {
        CurrentState = CurrentState.WithScheduler(CurrentState.Scheduler.Add(decisions));
    }

    public IGameEvent[] ExecuteScheduled(string? phase, string when)
    {
        // Enum parsing if 'when' is string?
        // Interface ExecuteScheduled(string, string).
        // DecisionTimingWhen is Enum.
        // Need to parse 'when' string to Enum or compare strings?
        // DecisionTimingWhen is likely used. Let's assume input string matches Enum name.

        if (!Enum.TryParse<DecisionTimingWhen>(when, out var whenEnum))
        {
            return []; // Or throw?
        }

        var toExecute = CurrentState.Scheduler.GetDecisions(d =>
            d.Timing.Phase == phase &&
            d.Timing.When == whenEnum).ToList();

        List<IGameEvent> allEvents = [];
        foreach (var decision in toExecute)
        {
            var decisionEvents = Apply(decision);
            if (decision.Timing.Frequency == DecisionTimingFrequency.Single)
            {
                CurrentState = CurrentState.WithScheduler(CurrentState.Scheduler.Remove(decision));
            }
            allEvents.AddRange(decisionEvents);
        }
        return allEvents.ToArray();
    }

    public IGameEvent[] Apply(IDecision decision)
    {
        var decisionType = decision.GetType();
        _logger?.Log($"[Orchestrator] Apply: {decisionType.Name}");
        if (_appliers.TryGetValue(decisionType, out var applier))
        {
            // Dynamic dispatch to IApplier<T>.Apply(T, GameState)
            var response = (ApplierResponse)((dynamic)applier).Apply((dynamic)decision, CurrentState);
            CurrentState = response.GameState;
            return response.GameEvents;
        }
        else
        {
            // Log missing applier?
            var msg = $"No applier registered for decision type {decisionType.Name}";
            _logger?.LogError(msg);
            throw new InvalidOperationException(msg);
        }
    }

    private IGameLogger? _logger;
    public void SetLogger(IGameLogger logger)
    {
        _logger = logger;
    }
}
