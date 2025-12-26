namespace TurnForge.Engine.Services.Dice.Modifiers;

/// <summary>
/// Keeps the N highest dice from a roll, dropping the rest.
/// </summary>
/// <param name="Count">Number of dice to keep.</param>
public record KeepHighestModifier(int Count) : IDiceModifier
{
    public ModifierResult Apply(IReadOnlyList<int> rolls, int diceSides, Random random)
    {
        if (Count <= 0)
            throw new ArgumentException("Count must be positive", nameof(Count));
        
        if (Count >= rolls.Count)
        {
            // Keep all dice
            var allKept = rolls.Select(RollHistoryEntry.Kept).ToList();
            return new ModifierResult(rolls.ToList(), allKept);
        }
        
        // Sort descending to find highest
        var sorted = rolls
            .Select((value, index) => (value, index))
            .OrderByDescending(x => x.value)
            .ToList();
        
        var keptIndices = sorted.Take(Count).Select(x => x.index).ToHashSet();
        
        var history = new List<RollHistoryEntry>();
        var finalRolls = new List<int>();
        
        for (int i = 0; i < rolls.Count; i++)
        {
            if (keptIndices.Contains(i))
            {
                finalRolls.Add(rolls[i]);
                history.Add(RollHistoryEntry.Kept(rolls[i]));
            }
            else
            {
                history.Add(RollHistoryEntry.Dropped(rolls[i]));
            }
        }
        
        return new ModifierResult(finalRolls, history);
    }
}
