using TurnForge.Engine.Services.Dice.Modifiers;
using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.Services.Dice;

/// <summary>
/// Default implementation of IDiceThrowService.
/// </summary>
/// <remarks>
/// Inject a seeded Random instance for deterministic testing:
/// <code>
/// var service = new DiceThrowService(new Random(42));
/// </code>
/// </remarks>
public sealed class DiceThrowService : IDiceThrowService
{
    private readonly Random _random;
    
    /// <summary>
    /// Creates a new DiceThrowService with optional Random instance.
    /// </summary>
    /// <param name="random">Random instance for dice rolls. If null, uses Random.Shared.</param>
    public DiceThrowService(Random? random = null)
    {
        _random = random ?? Random.Shared;
    }
    
    /// <inheritdoc />
    public DiceRollResult Roll(DiceThrowType diceThrow, bool trackHistory = false)
    {
        return RollInternal(diceThrow, null, trackHistory);
    }
    
    /// <inheritdoc />
    public DiceRollResult Roll(string notation, bool trackHistory = false)
    {
        var diceThrow = DiceThrowType.Parse(notation);
        return RollInternal(diceThrow, null, trackHistory);
    }
    
    /// <inheritdoc />
    public DiceRollResult Roll(DiceThrowType diceThrow, DiceThrowLimit limit, bool trackHistory = false)
    {
        return RollInternal(diceThrow, limit, trackHistory);
    }
    
    /// <inheritdoc />
    public DiceRollResult Roll(string diceNotation, string limitNotation, bool trackHistory = false)
    {
        var diceThrow = DiceThrowType.Parse(diceNotation);
        var limit = DiceThrowLimit.Parse(limitNotation);
        return RollInternal(diceThrow, limit, trackHistory);
    }
    
    // ─────────────────────────────────────────────────────────────
    // Internal Implementation
    // ─────────────────────────────────────────────────────────────
    
    private DiceRollResult RollInternal(DiceThrowType diceThrow, DiceThrowLimit? limit, bool trackHistory)
    {
        // 1. Roll initial dice
        var rawRolls = new List<int>();
        for (int i = 0; i < diceThrow.DiceCount; i++)
        {
            rawRolls.Add(_random.Next(1, diceThrow.DiceSides + 1));
        }
        
        // 2. Apply modifiers in order
        IReadOnlyList<int> currentRolls = rawRolls;
        var allHistory = new List<RollHistoryEntry>();
        
        foreach (var modifier in diceThrow.Modifiers)
        {
            var result = modifier.Apply(currentRolls, diceThrow.DiceSides, _random);
            currentRolls = result.FinalRolls;
            
            if (trackHistory)
            {
                allHistory.AddRange(result.History);
            }
        }
        
        // 3. If no modifiers and tracking history, add "Kept" for all
        if (diceThrow.Modifiers.Count == 0 && trackHistory)
        {
            allHistory.AddRange(currentRolls.Select(RollHistoryEntry.Kept));
        }
        
        // 4. Calculate total
        var diceSum = currentRolls.Sum();
        var total = diceSum + diceThrow.Modifier;
        
        return new DiceRollResult
        {
            Total = total,
            FinalRolls = currentRolls.ToList(),
            History = trackHistory ? allHistory : null,
            DiceThrow = diceThrow,
            Limit = limit
        };
    }
}
