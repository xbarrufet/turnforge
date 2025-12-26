using TurnForge.Engine.Decisions.Actions;
using TurnForge.Engine.Services.Dice.ValueObjects;
using TurnForge.Engine.Strategies.Interactions;

namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Result of executing an action strategy.
/// Supports three states: Completed, Suspended (for user input), Failed.
/// </summary>
/// <remarks>
/// Design Pattern: Result Type with Suspension
/// - Completed: IsValid=true, contains decisions to apply
/// - Suspended: Status=Suspended, contains InteractionRequest for UI
/// - Failed: IsValid=false, contains errors
/// 
/// For Interactive Pipelines:
/// When a strategy needs user input (dice roll, resource spend), it returns
/// Suspended with an InteractionRequest. The UI displays the appropriate
/// control and calls SubmitInteractionCommand to resume execution.
/// </remarks>
public record StrategyResult
{
    /// <summary>
    /// Execution status of the strategy.
    /// </summary>
    public StrategyStatus Status { get; init; }
    
    /// <summary>
    /// Whether the action completed successfully (shorthand for Status == Completed).
    /// </summary>
    public bool IsValid => Status == StrategyStatus.Completed;
    
    /// <summary>
    /// Whether the action is suspended waiting for user input.
    /// </summary>
    public bool IsSuspended => Status == StrategyStatus.Suspended;
    
    /// <summary>
    /// Decisions to apply if completed (empty if failed/suspended).
    /// </summary>
    public IReadOnlyList<ActionDecision> Decisions { get; init; } = Array.Empty<ActionDecision>();
    
    /// <summary>
    /// Validation error messages (empty if valid/suspended).
    /// </summary>
    public IReadOnlyList<string> ValidationErrors { get; init; } = Array.Empty<string>();
    
    /// <summary>
    /// Metadata for UI/analytics (cost, tags, description).
    /// </summary>
    public ActionMetadata Metadata { get; init; } = ActionMetadata.Empty;
    
    /// <summary>
    /// Dice rolls made during action execution (for UI animation).
    /// </summary>
    public IReadOnlyList<DiceRollResult>? Rolls { get; init; }
    
    /// <summary>
    /// Interaction request for UI when suspended.
    /// Only populated when Status == Suspended.
    /// </summary>
    public InteractionRequest? Interaction { get; init; }
    
    // === Factory Methods ===
    
    /// <summary>
    /// Create completed result with a single decision.
    /// </summary>
    public static StrategyResult Completed(ActionDecision decision)
    {
        return new StrategyResult
        {
            Status = StrategyStatus.Completed,
            Decisions = new[] { decision }
        };
    }
    
    /// <summary>
    /// Create completed result with multiple decisions.
    /// </summary>
    public static StrategyResult Completed(IEnumerable<ActionDecision> decisions)
    {
        return new StrategyResult
        {
            Status = StrategyStatus.Completed,
            Decisions = decisions.ToList()
        };
    }
    
    /// <summary>
    /// Create completed result with decisions and rolls.
    /// </summary>
    public static StrategyResult Completed(IEnumerable<ActionDecision> decisions, IEnumerable<DiceRollResult> rolls)
    {
        return new StrategyResult
        {
            Status = StrategyStatus.Completed,
            Decisions = decisions.ToList(),
            Rolls = rolls.ToList()
        };
    }
    
    /// <summary>
    /// Create suspended result waiting for user input.
    /// </summary>
    public static StrategyResult Suspended(InteractionRequest request)
    {
        return new StrategyResult
        {
            Status = StrategyStatus.Suspended,
            Interaction = request
        };
    }
    
    /// <summary>
    /// Create failed result with error message.
    /// </summary>
    public static StrategyResult Failed(string error)
    {
        return new StrategyResult
        {
            Status = StrategyStatus.Failed,
            ValidationErrors = new[] { error }
        };
    }
    
    /// <summary>
    /// Create failed result with multiple errors.
    /// </summary>
    public static StrategyResult Failed(IEnumerable<string> errors)
    {
        return new StrategyResult
        {
            Status = StrategyStatus.Failed,
            ValidationErrors = errors.ToList()
        };
    }
    
    // === Legacy Factory Methods (for backward compat) ===
    
    public static StrategyResult Success(ActionDecision decision) => Completed(decision);
    public static StrategyResult Success(IEnumerable<ActionDecision> decisions) => Completed(decisions);
    
    // === Fluent Methods ===
    
    public StrategyResult WithMetadata(ActionMetadata metadata)
    {
        return this with { Metadata = metadata };
    }
    
    public StrategyResult WithRolls(IEnumerable<DiceRollResult> rolls)
    {
        return this with { Rolls = rolls.ToList() };
    }
    
    public StrategyResult WithRoll(DiceRollResult roll)
    {
        var currentRolls = Rolls?.ToList() ?? new List<DiceRollResult>();
        currentRolls.Add(roll);
        return this with { Rolls = currentRolls };
    }
}

/// <summary>
/// Backward compatibility alias for StrategyResult.
/// </summary>
[Obsolete("Use StrategyResult instead")]
public record ActionStrategyResult : StrategyResult;
