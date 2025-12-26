namespace TurnForge.Engine.Services.Dice.Modifiers;

/// <summary>
/// Result from applying a dice modifier.
/// </summary>
/// <param name="FinalRolls">The dice values after applying the modifier.</param>
/// <param name="History">History entries describing what happened to each die.</param>
public record ModifierResult(
    IReadOnlyList<int> FinalRolls,
    IReadOnlyList<RollHistoryEntry> History
);
