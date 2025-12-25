namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Metadata about an action for UI feedback and analytics.
/// </summary>
/// <remarks>
/// Contains non-critical information about action execution.
/// Currently tracks Action Points cost. Additional fields can be added as needed.
/// </remarks>
public sealed record ActionMetadata
{
    /// <summary>
    /// Action Points cost of this action (0 for free actions).
    /// </summary>
    public int ActionPointsCost { get; init; }
    
    /// <summary>
    /// Empty metadata (no cost).
    /// </summary>
    public static ActionMetadata Empty => new();
}
