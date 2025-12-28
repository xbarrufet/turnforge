using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace BarelyAlive.Rules.Events;

/// <summary>
/// Event emitted after damage has been applied to an entity.
/// Contains the remaining health to allow death checks.
/// </summary>
public record DamageAppliedEvent(
    EntityId Target,
    EntityId Source,
    int Amount,
    int RemainingHealth,
    string Cause) : IWorkflowEvent;
