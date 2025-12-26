namespace TurnForge.Engine.Services.Dice.ValueObjects;

/// <summary>
/// Represents a pass/fail threshold for a dice roll (e.g., "10+" means >= 10).
/// </summary>
public record DiceThrowLimit
{
    /// <summary>The threshold value that must be met or exceeded.</summary>
    public int Threshold { get; init; }
    
    /// <summary>
    /// Checks if a total passes this limit (>= threshold).
    /// </summary>
    public bool IsPassed(int total) => total >= Threshold;
    
    /// <summary>
    /// Parses a limit notation string (e.g., "10+").
    /// </summary>
    /// <param name="notation">Notation like "10+", "15+", "5+"</param>
    /// <returns>Parsed DiceThrowLimit</returns>
    /// <exception cref="FormatException">If notation is invalid</exception>
    public static DiceThrowLimit Parse(string notation)
    {
        if (string.IsNullOrWhiteSpace(notation))
            throw new FormatException("Limit notation cannot be empty");
        
        notation = notation.Trim();
        
        if (!notation.EndsWith('+'))
            throw new FormatException($"Limit notation must end with '+': {notation}");
        
        var numberPart = notation[..^1]; // Remove trailing '+'
        
        if (!int.TryParse(numberPart, out var threshold))
            throw new FormatException($"Invalid threshold number: {numberPart}");
        
        if (threshold < 1)
            throw new FormatException($"Threshold must be positive: {threshold}");
        
        return new DiceThrowLimit { Threshold = threshold };
    }
    
    /// <summary>
    /// Tries to parse a limit notation string.
    /// </summary>
    public static bool TryParse(string notation, out DiceThrowLimit? result)
    {
        try
        {
            result = Parse(notation);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
    
    public override string ToString() => $"{Threshold}+";
}
