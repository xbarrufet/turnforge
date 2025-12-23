using System;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Results;

/// <summary>
/// Base record for game effects with common metadata.
/// Inherit from this to ensure consistent effect structure.
/// Provides automatic timestamp and origin tracking.
/// </summary>
public abstract record GameEffect : IGameEffect
{
    public EffectOrigin Origin { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public abstract string Description { get; }
    
    protected GameEffect(EffectOrigin origin)
    {
        Origin = origin;
    }
}
