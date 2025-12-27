using System;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace TurnForge.Engine.Appliers.Entity;

/// <summary>
/// Result of an Applier execution.
/// Contains the new immutable state and any generated events.
/// </summary>
public sealed record ApplierResponse
{
    public GameState GameState { get; init; }
    
    /// <summary>
    /// Events generated during application (for UI/Logging).
    /// </summary>
    public IGameEvent[] GameEvents { get; init; }
    
    public ApplierResponse(GameState gameState, IGameEvent[] gameEvents = null)
    {
        GameState = gameState;
        GameEvents = gameEvents ?? Array.Empty<IGameEvent>();
    }
}