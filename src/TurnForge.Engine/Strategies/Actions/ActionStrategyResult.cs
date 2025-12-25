using TurnForge.Engine.Decisions.Actions;

namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Result of executing an action strategy.
/// Contains validation outcome, decisions to apply, and metadata for UI.
/// </summary>
/// <remarks>
/// Design Pattern: Result Type (Railway Oriented Programming)
/// - Success path: IsValid=true, contains decisions
/// - Failure path: IsValid=false, contains errors
/// 
/// Metadata is included even on failure to provide feedback 
/// (e.g., "You need 3 AP but only have 1").
/// </remarks>
public sealed record ActionStrategyResult
{
    /// <summary>
    /// Whether the action passed validation.
    /// </summary>
    public bool IsValid { get; init; }
    
    /// <summary>
    /// Decisions to apply if valid (empty if failed).
    /// </summary>
    public IReadOnlyList<ActionDecision> Decisions { get; init; } = Array.Empty<ActionDecision>();
    
    /// <summary>
    /// Validation error messages (empty if valid).
    /// </summary>
    public IReadOnlyList<string> ValidationErrors { get; init; } = Array.Empty<string>();
    
    /// <summary>
    /// Metadata for UI/analytics (cost, tags, description).
    /// Present even on failure for feedback purposes.
    /// </summary>
    public ActionMetadata Metadata { get; init; } = ActionMetadata.Empty;
    
    // === Factory Methods ===
    
    /// <summary>
    /// Create successful result with a single decision.
    /// </summary>
    public static ActionStrategyResult Success(ActionDecision decision)
    {
        return new ActionStrategyResult
        {
            IsValid = true,
            Decisions = new[] { decision }
        };
    }
    
    /// <summary>
    /// Create successful result with multiple decisions.
    /// </summary>
    public static ActionStrategyResult Success(IEnumerable<ActionDecision> decisions)
    {
        return new ActionStrategyResult
        {
            IsValid = true,
            Decisions = decisions.ToList()
        };
    }
    
    /// <summary>
    /// Create failed result with error message.
    /// </summary>
    public static ActionStrategyResult Failed(string error)
    {
        return new ActionStrategyResult
        {
            IsValid = false,
            ValidationErrors = new[] { error }
        };
    }
    
    /// <summary>
    /// Create failed result with multiple errors.
    /// </summary>
    public static ActionStrategyResult Failed(IEnumerable<string> errors)
    {
        return new ActionStrategyResult
        {
            IsValid = false,
            ValidationErrors = errors.ToList()
        };
    }
    
    /// <summary>
    /// Add metadata to this result (fluent API).
    /// </summary>
    public ActionStrategyResult WithMetadata(ActionMetadata metadata)
    {
        return this with { Metadata = metadata };
    }
}
