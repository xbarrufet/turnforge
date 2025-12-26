namespace TurnForge.Engine.Services.Dice.Modifiers;

/// <summary>
/// Rerolls dice that are at or below a threshold value.
/// </summary>
/// <param name="Threshold">Dice at or below this value will be rerolled.</param>
/// <param name="MaxTimes">Maximum number of times to reroll each die (default 1).</param>
public record RerollModifier(int Threshold, int MaxTimes = 1) : IDiceModifier
{
    public ModifierResult Apply(IReadOnlyList<int> rolls, int diceSides, Random random)
    {
        if (Threshold < 1)
            throw new ArgumentException("Threshold must be at least 1", nameof(Threshold));
        
        if (MaxTimes < 1)
            throw new ArgumentException("MaxTimes must be at least 1", nameof(MaxTimes));
        
        var history = new List<RollHistoryEntry>();
        var finalRolls = new List<int>();
        
        foreach (var originalValue in rolls)
        {
            var currentValue = originalValue;
            var rerollCount = 0;
            
            // Reroll while at or below threshold, up to MaxTimes
            while (currentValue <= Threshold && rerollCount < MaxTimes)
            {
                var newValue = random.Next(1, diceSides + 1);
                rerollCount++;
                
                if (newValue > Threshold || rerollCount >= MaxTimes)
                {
                    // Final value found (either passed threshold or max rerolls reached)
                    history.Add(RollHistoryEntry.Rerolled(originalValue, newValue));
                    currentValue = newValue;
                }
                else
                {
                    // Still below threshold, continue rerolling
                    currentValue = newValue;
                }
            }
            
            if (rerollCount == 0)
            {
                // No reroll needed, value was already above threshold
                history.Add(RollHistoryEntry.Kept(originalValue));
            }
            
            finalRolls.Add(currentValue);
        }
        
        return new ModifierResult(finalRolls, history);
    }
}
