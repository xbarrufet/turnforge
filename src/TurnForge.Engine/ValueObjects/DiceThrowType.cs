using System;
using System.Text.RegularExpressions;

namespace TurnForge.Engine.Values;

/// <summary>
/// Value object representing a dice roll formula (e.g., "2D6+3").
/// Immutable and parseable from string notation.
/// </summary>
public record DiceThrowType
{
    public int DiceCount { get; init; }
    public int DiceSides { get; init; }
    public int Modifier { get; init; }

    public override string ToString() => 
        Modifier == 0 
            ? $"{DiceCount}D{DiceSides}" 
            : $"{DiceCount}D{DiceSides}{(Modifier > 0 ? "+" : "")}{Modifier}";

    /// <summary>
    /// Parses a dice notation string (e.g. "1D6", "2D10+5", "1D20-1").
    /// </summary>
    public static DiceThrowType Parse(string notation)
    {
        if (string.IsNullOrWhiteSpace(notation))
            throw new ArgumentException("Dice notation cannot be empty", nameof(notation));

        // Regex for NdS formatted dice, optional numeric modifier
        // Groups: 1=Count, 2=Sides, 3=Modifier (optional)
        var match = Regex.Match(notation.ToUpper(), @"^(\d+)D(\d+)([+-]\d+)?$");

        if (!match.Success)
            throw new FormatException($"Invalid dice notation '{notation}'. Expected format like '1D6' or '2D6+3'.");

        int count = int.Parse(match.Groups[1].Value);
        int sides = int.Parse(match.Groups[2].Value);
        int mod = 0;

        if (match.Groups[3].Success)
        {
            mod = int.Parse(match.Groups[3].Value);
        }

        return new DiceThrowType 
        { 
            DiceCount = count, 
            DiceSides = sides, 
            Modifier = mod 
        };
    }
}
