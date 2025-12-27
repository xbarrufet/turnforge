using TurnForge.Engine.Traits.Standard.Checkers;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Represents a stat that can be "checked" with dice rolls.
/// Examples: ToHit, ToWound, Morale, Leadership.
/// </summary>
/// <remarks>
/// This trait is data-only. The actual check logic is in IDiceCheckService.
/// 
/// Example usage:
/// <code>
/// var toHit = new CheckerStatTrait("ToHit", "1d6", new FixedThreshold(4));
/// var toWound = new CheckerStatTrait("ToWound", "1d6", new TableLookup("ToWound"));
/// var morale = new CheckerStatTrait("Morale", "2d6", new OpposedCheck());
/// </code>
/// </remarks>
public class CheckerStatTrait : BaseTrait
{
    /// <summary>
    /// Name of this stat (e.g., "ToHit", "ToWound", "Morale").
    /// </summary>
    public string StatName { get; }
    
    /// <summary>
    /// Dice pattern to roll (e.g., "1d6", "2d6").
    /// </summary>
    public PotentialRandomValue DicePattern { get; }
    
    /// <summary>
    /// How to validate the roll result.
    /// </summary>
    public ICheckCondition Condition { get; }
    
    public CheckerStatTrait(string statName, PotentialRandomValue dicePattern, ICheckCondition condition)
    {
        StatName = statName;
        DicePattern = dicePattern;
        Condition = condition;
    }
    
    // ─────────────────────────────────────────────────────────────
    // Factory Methods
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Creates a stat with a fixed threshold (e.g., "4+").
    /// </summary>
    public static CheckerStatTrait Fixed(string name, PotentialRandomValue dice, int threshold)
        => new(name, dice, new FixedThreshold(threshold));
    
    /// <summary>
    /// Creates a stat that checks against an opposed value.
    /// </summary>
    public static CheckerStatTrait Opposed(string name, PotentialRandomValue dice)
        => new(name, dice, new OpposedCheck());
    
    /// <summary>
    /// Creates a stat that uses table lookup.
    /// </summary>
    public static CheckerStatTrait Table(string name, PotentialRandomValue dice, string tableName)
        => new(name, dice, new TableLookup(tableName));
}
