namespace TurnForge.Engine.Services.Rulebook;

/// <summary>
/// Service for looking up rules tables (e.g., ToWound, ArmorSave).
/// Tables define thresholds based on stat comparisons.
/// </summary>
/// <remarks>
/// This is a placeholder interface. Concrete implementations will
/// load table data from JSON/resources specific to each game ruleset.
/// </remarks>
public interface IRulebookService
{
    /// <summary>
    /// Checks if a roll meets the threshold from a table lookup.
    /// </summary>
    /// <param name="tableName">Table name (e.g., "ToWound")</param>
    /// <param name="attackerValue">Attacker stat (e.g., Strength)</param>
    /// <param name="defenderValue">Defender stat (e.g., Toughness)</param>
    /// <param name="rollValue">Actual roll result</param>
    /// <returns>True if roll meets or exceeds threshold</returns>
    bool TableCheck(string tableName, int attackerValue, int defenderValue, int rollValue);
    
    /// <summary>
    /// Gets the required threshold from a table lookup.
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <param name="attackerValue">Attacker stat</param>
    /// <param name="defenderValue">Defender stat</param>
    /// <returns>Required threshold (e.g., 4 for "4+")</returns>
    int GetThreshold(string tableName, int attackerValue, int defenderValue);
    
    /// <summary>
    /// Simple threshold check (for non-opposed checks).
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <param name="value">Single lookup value</param>
    /// <param name="rollValue">Actual roll result</param>
    /// <returns>True if roll meets or exceeds threshold</returns>
    bool TableCheck(string tableName, int value, int rollValue);
    
    /// <summary>
    /// Gets threshold for simple (non-opposed) table lookup.
    /// </summary>
    int GetThreshold(string tableName, int value);
}
