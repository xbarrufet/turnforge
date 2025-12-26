using System.Text.RegularExpressions;
using TurnForge.Engine.Services.Dice.Modifiers;
using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.Services.Dice.Parsing;

/// <summary>
/// Parses dice notation strings into DiceThrowType objects.
/// </summary>
/// <remarks>
/// Supported notation:
/// - Basic: 1D20, 2D6, 3D6+5, 2D4-1
/// - Keep highest: 3D6kh2 (roll 3, keep 2 highest)
/// - Keep lowest: 4D6kl3 (roll 4, keep 3 lowest)
/// - Reroll: 2D6r1 (reroll 1s once)
/// - Combined: 4D6kh3r1+2
/// </remarks>
public static class DiceNotationParser
{
    // Pattern: [count]D[sides][kh#|kl#][r#][+|-modifier]
    // Examples: 1D20, 3D6kh2, 4D6kl3+2, 2D6r1
    private static readonly Regex DicePattern = new(
        @"^(\d+)?[Dd](\d+)(?:kh(\d+))?(?:kl(\d+))?(?:r(\d+))?([+-]\d+)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    
    /// <summary>
    /// Parses a dice notation string into a DiceThrowType.
    /// </summary>
    /// <exception cref="FormatException">If notation is invalid.</exception>
    public static DiceThrowType Parse(string notation)
    {
        if (string.IsNullOrWhiteSpace(notation))
            throw new FormatException("Dice notation cannot be empty");
        
        notation = notation.Trim().Replace(" ", "");
        
        var match = DicePattern.Match(notation);
        if (!match.Success)
            throw new FormatException($"Invalid dice notation: {notation}");
        
        // Group 1: dice count (optional, default 1)
        var diceCount = string.IsNullOrEmpty(match.Groups[1].Value) 
            ? 1 
            : int.Parse(match.Groups[1].Value);
        
        // Group 2: dice sides (required)
        var diceSides = int.Parse(match.Groups[2].Value);
        
        // Validate
        if (diceCount < 1)
            throw new FormatException($"Dice count must be positive: {diceCount}");
        if (diceSides < 2)
            throw new FormatException($"Dice must have at least 2 sides: {diceSides}");
        
        // Build modifiers list
        var modifiers = new List<IDiceModifier>();
        
        // Group 3: keep highest
        if (!string.IsNullOrEmpty(match.Groups[3].Value))
        {
            var keepCount = int.Parse(match.Groups[3].Value);
            if (keepCount < 1 || keepCount > diceCount)
                throw new FormatException($"Keep highest count must be 1-{diceCount}: {keepCount}");
            modifiers.Add(new KeepHighestModifier(keepCount));
        }
        
        // Group 4: keep lowest
        if (!string.IsNullOrEmpty(match.Groups[4].Value))
        {
            var keepCount = int.Parse(match.Groups[4].Value);
            if (keepCount < 1 || keepCount > diceCount)
                throw new FormatException($"Keep lowest count must be 1-{diceCount}: {keepCount}");
            modifiers.Add(new KeepLowestModifier(keepCount));
        }
        
        // Group 5: reroll threshold
        if (!string.IsNullOrEmpty(match.Groups[5].Value))
        {
            var threshold = int.Parse(match.Groups[5].Value);
            if (threshold < 1 || threshold >= diceSides)
                throw new FormatException($"Reroll threshold must be 1-{diceSides - 1}: {threshold}");
            modifiers.Add(new RerollModifier(threshold));
        }
        
        // Group 6: arithmetic modifier
        var modifier = string.IsNullOrEmpty(match.Groups[6].Value)
            ? 0
            : int.Parse(match.Groups[6].Value);
        
        return new DiceThrowType
        {
            DiceCount = diceCount,
            DiceSides = diceSides,
            Modifier = modifier,
            Modifiers = modifiers
        };
    }
    
    /// <summary>
    /// Tries to parse a dice notation string.
    /// </summary>
    public static bool TryParse(string notation, out DiceThrowType? result)
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
}
