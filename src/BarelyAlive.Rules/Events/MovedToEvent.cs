using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Events;

/// <summary>
/// Event emitted when an entity moves to a new position.
/// Triggers reactive traits like ExplosiveTrait (traps).
/// </summary>
public record MovedToEvent(EntityId AgentId, Position NewPosition) : IWorkflowEvent;
