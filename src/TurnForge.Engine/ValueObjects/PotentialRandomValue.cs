using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.Dice.Parsing;
using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.ValueObjects;

/// <summary>
/// Represents a value that can be either a fixed number or a random value (dice roll).
/// </summary>
/// <remarks>
/// Useful for stats, damage, healing, movement points, etc. that can be:
/// - Fixed value: 5 (always returns 5)
/// - Dice notation: "2d6+3" (returns roll result when resolved)
/// 
/// Example usage:
/// <code>
/// var damage = PotentialRandomValue.Fixed(5);        // Always 5
/// var damage = PotentialRandomValue.Dice("2d6+3");   // 2d6+3 roll
/// var damage = PotentialRandomValue.Parse("2d6+3");  // Auto-detect
/// var damage = PotentialRandomValue.Parse("5");      // Fixed 5
/// 
/// int value = damage.Resolve(diceService);  // Resolves to actual value
/// </code>
/// </remarks>
public readonly record struct PotentialRandomValue
{
    /// <summary>
    /// The fixed value, used when IsFixed is true.
    /// </summary>
    public int FixedValue { get; }
    
    /// <summary>
    /// The dice configuration, used when IsFixed is false.
    /// </summary>
    public DiceThrowType? DiceThrow { get; }
    
    /// <summary>
    /// True if this is a fixed value, false if it requires a dice roll.
    /// </summary>
    public bool IsFixed => DiceThrow == null;
    
    /// <summary>
    /// True if this is a random value (dice roll), false if fixed.
    /// </summary>
    public bool IsRandom => DiceThrow != null;

    // ─────────────────────────────────────────────────────────────
    // Constructors (private, use factory methods)
    // ─────────────────────────────────────────────────────────────
    
    private PotentialRandomValue(int fixedValue)
    {
        FixedValue = fixedValue;
        DiceThrow = null;
    }
    
    private PotentialRandomValue(DiceThrowType diceThrow)
    {
        DiceThrow = diceThrow ?? throw new ArgumentNullException(nameof(diceThrow));
        FixedValue = 0;
    }
    
    // ─────────────────────────────────────────────────────────────
    // Factory Methods
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Creates a fixed value.
    /// </summary>
    public static PotentialRandomValue Fixed(int value) => new(value);
    
    /// <summary>
    /// Creates a random value from dice notation.
    /// </summary>
    /// <param name="notation">Dice notation (e.g., "2d6", "3d6+5", "4d6kh3").</param>
    public static PotentialRandomValue Dice(string notation) => 
        new(DiceThrowType.Parse(notation));
    
    /// <summary>
    /// Creates a random value from a DiceThrowType.
    /// </summary>
    public static PotentialRandomValue Dice(DiceThrowType diceThrow) => 
        new(diceThrow);
    
    /// <summary>
    /// Parses a string that can be either a fixed value or dice notation.
    /// </summary>
    /// <param name="value">Fixed number or dice notation (e.g., "5", "2d6+3").</param>
    /// <returns>PotentialRandomValue representing the parsed value.</returns>
    public static PotentialRandomValue Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));
        
        value = value.Trim();
        
        // Try to parse as integer first
        if (int.TryParse(value, out var fixedValue))
            return Fixed(fixedValue);
        
        // Try to parse as dice notation
        if (DiceNotationParser.TryParse(value, out var diceThrow) && diceThrow != null)
            return Dice(diceThrow);
        
        throw new FormatException($"Cannot parse '{value}' as fixed value or dice notation");
    }
    
    /// <summary>
    /// Tries to parse a string as a PotentialRandomValue.
    /// </summary>
    public static bool TryParse(string value, out PotentialRandomValue result)
    {
        try
        {
            result = Parse(value);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
    
    // ─────────────────────────────────────────────────────────────
    // Resolution
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Resolves this value to an actual integer.
    /// Fixed values return immediately; dice values are rolled.
    /// </summary>
    /// <param name="diceService">Required for random values.</param>
    /// <returns>The resolved integer value.</returns>
    public int Resolve(IDiceThrowService diceService)
    {
        if (IsFixed)
            return FixedValue;
        
        if (diceService == null)
            throw new ArgumentNullException(nameof(diceService), 
                "DiceThrowService required for random values");
        
        return diceService.Roll(DiceThrow!).Total;
    }
    
    /// <summary>
    /// Resolves this value, returning both the result and roll details (if applicable).
    /// </summary>
    /// <param name="diceService">Required for random values.</param>
    /// <returns>Resolution result with value and optional roll details.</returns>
    public ResolvedValue ResolveWithDetails(IDiceThrowService diceService)
    {
        if (IsFixed)
            return new ResolvedValue(FixedValue, null);
        
        if (diceService == null)
            throw new ArgumentNullException(nameof(diceService));
        
        var rollResult = diceService.Roll(DiceThrow!, trackHistory: true);
        return new ResolvedValue(rollResult.Total, rollResult);
    }
    
    // ─────────────────────────────────────────────────────────────
    // Utility
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Gets the minimum possible value.
    /// </summary>
    public int MinValue => IsFixed 
        ? FixedValue 
        : DiceThrow!.DiceCount + DiceThrow.Modifier;
    
    /// <summary>
    /// Gets the maximum possible value.
    /// </summary>
    public int MaxValue => IsFixed 
        ? FixedValue 
        : (DiceThrow!.DiceCount * DiceThrow.DiceSides) + DiceThrow.Modifier;
    
    /// <summary>
    /// Gets the average expected value.
    /// </summary>
    public double AverageValue => IsFixed 
        ? FixedValue 
        : (DiceThrow!.DiceCount * (DiceThrow.DiceSides + 1) / 2.0) + DiceThrow.Modifier;
    
    public override string ToString() => IsFixed 
        ? FixedValue.ToString() 
        : DiceThrow!.ToString();
    
    // ─────────────────────────────────────────────────────────────
    // Implicit Conversions
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Implicit conversion from int to fixed PotentialRandomValue.
    /// </summary>
    public static implicit operator PotentialRandomValue(int value) => Fixed(value);
    
    /// <summary>
    /// Implicit conversion from string (dice notation or fixed value).
    /// </summary>
    public static implicit operator PotentialRandomValue(string value) => Parse(value);
}

/// <summary>
/// Result of resolving a PotentialRandomValue.
/// </summary>
/// <param name="Value">The resolved integer value.</param>
/// <param name="RollResult">The dice roll result, if this was a random value.</param>
public readonly record struct ResolvedValue(int Value, DiceRollResult? RollResult)
{
    /// <summary>
    /// True if this value came from a dice roll.
    /// </summary>
    public bool WasRolled => RollResult != null;
    
    /// <summary>
    /// Implicit conversion to int.
    /// </summary>
    public static implicit operator int(ResolvedValue resolved) => resolved.Value;
}
