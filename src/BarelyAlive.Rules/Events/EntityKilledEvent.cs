using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Events;

/// <summary>
/// Event emitted when an entity is killed/destroyed.
/// </summary>
public record EntityKilledEvent(
    EntityId Target,
    EntityId? Killer,
    string Cause) : IWorkflowEvent;
