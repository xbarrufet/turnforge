using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.Rulebook;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.Traits.Standard.Checkers;

namespace TurnForge.Engine.Services.DiceCheck;

/// <summary>
/// Parameters for stat check resolution.
/// </summary>
/// <param name="OpposedValue">Value to check against for OpposedCheck</param>
/// <param name="AttackerValue">Attacker stat for TableLookup</param>
/// <param name="DefenderValue">Defender stat for TableLookup</param>
/// <param name="Rulebook">Service for table lookups (required for TableLookup)</param>
public record CheckParams(
    int? OpposedValue = null,
    int? AttackerValue = null,
    int? DefenderValue = null,
    IRulebookService? Rulebook = null
);

/// <summary>
/// Result of a stat check.
/// </summary>
/// <param name="Success">Whether the check passed</param>
/// <param name="RollValue">The actual roll result</param>
/// <param name="RequiredThreshold">The threshold that was needed</param>
/// <param name="StatName">Name of the stat that was checked</param>
public record CheckResult(
    bool Success,
    int RollValue,
    int RequiredThreshold,
    string StatName
)
{
    /// <summary>
    /// Margin by which the check passed/failed.
    /// Positive = passed by that much, Negative = failed by that much.
    /// </summary>
    public int Margin => RollValue - RequiredThreshold;
}

/// <summary>
/// Service for resolving stat checks with dice.
/// </summary>
public interface IDiceCheckService
{
    /// <summary>
    /// Performs a stat check.
    /// </summary>
    /// <param name="stat">The stat trait defining dice and condition</param>
    /// <param name="parameters">Context parameters for the check</param>
    /// <returns>Result of the check</returns>
    CheckResult Check(CheckerStatTrait stat, CheckParams parameters);
}
