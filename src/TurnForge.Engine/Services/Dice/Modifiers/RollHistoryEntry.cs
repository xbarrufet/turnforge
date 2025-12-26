namespace TurnForge.Engine.Services.Dice.Modifiers;

/// <summary>
/// Represents a single roll history entry for tracking dice modifications.
/// </summary>
/// <param name="OriginalValue">The original value rolled on the die.</param>
/// <param name="FinalValue">The final value after modifications (same as original if kept as-is).</param>
/// <param name="Reason">Description of what happened: "Kept", "Dropped", "Rerolled", etc.</param>
public record RollHistoryEntry(
    int OriginalValue,
    int FinalValue,
    string Reason
)
{
    /// <summary>
    /// Creates a history entry for a die that was kept without modification.
    /// </summary>
    public static RollHistoryEntry Kept(int value) => new(value, value, "Kept");
    
    /// <summary>
    /// Creates a history entry for a die that was dropped (not included in final rolls).
    /// </summary>
    public static RollHistoryEntry Dropped(int value) => new(value, value, "Dropped");
    
    /// <summary>
    /// Creates a history entry for a die that was rerolled.
    /// </summary>
    public static RollHistoryEntry Rerolled(int original, int newValue) => 
        new(original, newValue, $"Rerolled {original}→{newValue}");
}
