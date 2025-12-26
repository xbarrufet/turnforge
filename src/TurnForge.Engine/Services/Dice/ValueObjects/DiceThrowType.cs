using TurnForge.Engine.Services.Dice.Modifiers;

namespace TurnForge.Engine.Services.Dice.ValueObjects;

/// <summary>
/// Represents a dice throw notation (e.g., "3D6kh2+5").
/// </summary>
/// <remarks>
/// Immutable value object that encapsulates dice configuration and modifiers.
/// Can be created via Parse() or fluent builder methods.
/// </remarks>
public record DiceThrowType
{
    /// <summary>Number of dice to roll (e.g., 3 in "3D6").</summary>
    public int DiceCount { get; init; }
    
    /// <summary>Number of sides per die (e.g., 6 in "3D6").</summary>
    public int DiceSides { get; init; }
    
    /// <summary>Arithmetic modifier to add to total (e.g., +5 in "3D6+5").</summary>
    public int Modifier { get; init; }
    
    /// <summary>List of dice modifiers to apply (keep highest, reroll, etc.).</summary>
    public IReadOnlyList<IDiceModifier> Modifiers { get; init; } = [];
    
    /// <summary>
    /// Parses a dice notation string into a DiceThrowType.
    /// </summary>
    /// <param name="notation">Notation like "3D6", "2D20kh1", "4D6kl3+2", "2D6r1"</param>
    /// <returns>Parsed DiceThrowType</returns>
    /// <exception cref="FormatException">If notation is invalid</exception>
    public static DiceThrowType Parse(string notation)
    {
        return Parsing.DiceNotationParser.Parse(notation);
    }
    
    /// <summary>
    /// Tries to parse a dice notation string.
    /// </summary>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParse(string notation, out DiceThrowType? result)
    {
        return Parsing.DiceNotationParser.TryParse(notation, out result);
    }
    
    // ─────────────────────────────────────────────────────────────
    // Fluent Builders
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Adds a KeepHighest modifier.
    /// </summary>
    /// <param name="count">Number of dice to keep.</param>
    public DiceThrowType KeepHighest(int count) => 
        this with { Modifiers = [..Modifiers, new KeepHighestModifier(count)] };
    
    /// <summary>
    /// Adds a KeepLowest modifier.
    /// </summary>
    /// <param name="count">Number of dice to keep.</param>
    public DiceThrowType KeepLowest(int count) => 
        this with { Modifiers = [..Modifiers, new KeepLowestModifier(count)] };
    
    /// <summary>
    /// Adds a Reroll modifier.
    /// </summary>
    /// <param name="threshold">Reroll dice at or below this value.</param>
    /// <param name="maxTimes">Maximum rerolls per die (default 1).</param>
    public DiceThrowType Reroll(int threshold, int maxTimes = 1) => 
        this with { Modifiers = [..Modifiers, new RerollModifier(threshold, maxTimes)] };
    
    /// <summary>
    /// Returns the standard notation string for this dice throw.
    /// </summary>
    public override string ToString()
    {
        var result = $"{DiceCount}D{DiceSides}";
        
        foreach (var mod in Modifiers)
        {
            result += mod switch
            {
                KeepHighestModifier kh => $"kh{kh.Count}",
                KeepLowestModifier kl => $"kl{kl.Count}",
                RerollModifier r => $"r{r.Threshold}",
                _ => ""
            };
        }
        
        if (Modifier > 0)
            result += $"+{Modifier}";
        else if (Modifier < 0)
            result += Modifier.ToString();
        
        return result;
    }
}
