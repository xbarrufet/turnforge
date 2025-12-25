using System;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace TurnForge.Engine.Appliers.Entity.Results;

/// <summary>
/// Base record for game events with common metadata.
/// Inherit from this to ensure consistent event structure.
/// Provides automatic timestamp and origin tracking.
/// </summary>
public abstract record GameEvent : IGameEvent
{
    public EventOrigin Origin { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public abstract string Description { get; }
    
    protected GameEvent(EventOrigin origin)
    {
        Origin = origin;
    }
}
