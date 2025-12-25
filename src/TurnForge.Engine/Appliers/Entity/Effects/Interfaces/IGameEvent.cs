using System;
using TurnForge.Engine.Appliers.Entity.Results;

namespace TurnForge.Engine.Appliers.Entity.Results.Interfaces;

/// <summary>
/// Base interface for all game events.
/// Events carry immutable metadata about state changes for UI/logging/replay.
/// </summary>
public interface IGameEvent
{
    /// <summary>
    /// Origin/source of this event (Command, PhaseTransition, etc.).
    /// Enables UI to filter and display events appropriately.
    /// </summary>
    EventOrigin Origin { get; }
    
    /// <summary>
    /// Timestamp when event was generated (UTC).
    /// Enables replay, debugging, and audit logs.
    /// </summary>
    DateTime Timestamp { get; }
    
    /// <summary>
    /// Human-readable description of the event.
    /// Used for logging and debugging without inspecting event type.
    /// </summary>
    string Description { get; }
}
