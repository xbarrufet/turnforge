using System;
using TurnForge.Engine.Entities.Appliers.Results;

namespace TurnForge.Engine.Entities.Appliers.Results.Interfaces;

/// <summary>
/// Base interface for all game effects.
/// Effects carry metadata about state changes for UI/logging/replay.
/// </summary>
public interface IGameEffect
{
    /// <summary>
    /// Origin/source of this effect (Command, PhaseTransition, etc.).
    /// Enables UI to filter and display effects appropriately.
    /// </summary>
    EffectOrigin Origin { get; }
    
    /// <summary>
    /// Timestamp when effect was generated (UTC).
    /// Enables replay, debugging, and audit logs.
    /// </summary>
    DateTime Timestamp { get; }
    
    /// <summary>
    /// Human-readable description of the effect.
    /// Used for logging and debugging without inspecting effect type.
    /// </summary>
    string Description { get; }
}