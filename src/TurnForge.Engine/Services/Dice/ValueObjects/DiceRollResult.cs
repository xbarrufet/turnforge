using TurnForge.Engine.Services.Dice.Modifiers;

namespace TurnForge.Engine.Services.Dice.ValueObjects;

/// <summary>
/// Result of a dice roll including total, individual rolls, and optional history.
/// </summary>
public record DiceRollResult
{
    /// <summary>Final sum of all kept dice plus modifier.</summary>
    public int Total { get; init; }
    
    /// <summary>Final dice values after all modifiers applied.</summary>
    public IReadOnlyList<int> FinalRolls { get; init; } = [];
    
    /// <summary>
    /// History of what happened to each die (only populated if trackHistory=true).
    /// </summary>
    public IReadOnlyList<RollHistoryEntry>? History { get; init; }
    
    /// <summary>The original dice throw configuration.</summary>
    public DiceThrowType DiceThrow { get; init; } = null!;
    
    /// <summary>The limit used for pass/fail check (null if no limit).</summary>
    public DiceThrowLimit? Limit { get; init; }
    
    /// <summary>
    /// Whether the roll passed the limit check.
    /// Null if no limit was specified.
    /// </summary>
    public bool? Pass => Limit?.IsPassed(Total);
    
    // ─────────────────────────────────────────────────────────────
    // Fluent Comparisons (for use without explicit Limit)
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>Checks if total is greater than or equal to threshold.</summary>
    public bool IsHigherOrEqualThan(int threshold) => Total >= threshold;
    
    /// <summary>Checks if total is less than threshold.</summary>
    public bool IsLowerThan(int threshold) => Total < threshold;
    
    /// <summary>Checks if total equals exactly the value.</summary>
    public bool IsExactly(int value) => Total == value;
    
    public override string ToString()
    {
        var rollsStr = string.Join(", ", FinalRolls);
        var result = $"[{rollsStr}] = {Total}";
        
        if (Pass.HasValue)
            result += Pass.Value ? " ✓ PASS" : " ✗ FAIL";
        
        return result;
    }
}
