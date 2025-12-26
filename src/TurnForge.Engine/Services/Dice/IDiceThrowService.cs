using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.Services.Dice;

/// <summary>
/// Service for rolling dice with support for modifiers and pass/fail checks.
/// </summary>
/// <remarks>
/// Supports standard RPG dice notation (e.g., "3D6kh2+5") with:
/// - Keep highest/lowest modifiers
/// - Reroll thresholds
/// - Arithmetic modifiers
/// - Pass/fail limit checks
/// 
/// Inject Random instance for deterministic testing.
/// </remarks>
public interface IDiceThrowService
{
    /// <summary>
    /// Rolls dice according to the specified configuration.
    /// </summary>
    /// <param name="diceThrow">Dice configuration.</param>
    /// <param name="trackHistory">If true, populates History in result.</param>
    DiceRollResult Roll(DiceThrowType diceThrow, bool trackHistory = false);
    
    /// <summary>
    /// Parses notation and rolls dice.
    /// </summary>
    /// <param name="notation">Dice notation (e.g., "3D6+5", "2D20kh1").</param>
    /// <param name="trackHistory">If true, populates History in result.</param>
    DiceRollResult Roll(string notation, bool trackHistory = false);
    
    /// <summary>
    /// Rolls dice with a pass/fail limit check.
    /// </summary>
    /// <param name="diceThrow">Dice configuration.</param>
    /// <param name="limit">Pass threshold.</param>
    /// <param name="trackHistory">If true, populates History in result.</param>
    DiceRollResult Roll(DiceThrowType diceThrow, DiceThrowLimit limit, bool trackHistory = false);
    
    /// <summary>
    /// Parses notation and rolls dice with a limit check.
    /// </summary>
    /// <param name="diceNotation">Dice notation (e.g., "1D6").</param>
    /// <param name="limitNotation">Limit notation (e.g., "4+").</param>
    /// <param name="trackHistory">If true, populates History in result.</param>
    DiceRollResult Roll(string diceNotation, string limitNotation, bool trackHistory = false);
}
