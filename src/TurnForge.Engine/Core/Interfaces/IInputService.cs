namespace TurnForge.Engine.Core.Interfaces;

/// <summary>
/// Service to abstract user input requests (e.g., dice rolls, choices).
/// </summary>
public interface IInputService
{
    /// <summary>
    /// Requests a dice roll from the user (or simulates it).
    /// </summary>
    /// <param name="reason">Why the roll is needed (e.g., "DarkZone check")</param>
    /// <param name="dicePattern">Dice to roll (e.g., "1d6")</param>
    /// <returns>The total result of the roll.</returns>
    int RequestDiceRoll(string reason, string dicePattern);
}
