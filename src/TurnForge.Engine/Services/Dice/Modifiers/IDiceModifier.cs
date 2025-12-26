namespace TurnForge.Engine.Services.Dice.Modifiers;

/// <summary>
/// Interface for dice modifiers that manipulate roll results.
/// </summary>
/// <remarks>
/// Modifiers are applied in order to raw roll results.
/// Examples: keep highest, keep lowest, reroll specific values.
/// </remarks>
public interface IDiceModifier
{
    /// <summary>
    /// Applies this modifier to a set of dice rolls.
    /// </summary>
    /// <param name="rolls">The current dice values.</param>
    /// <param name="diceSides">Number of sides on the dice (for rerolls).</param>
    /// <param name="random">Random instance for any new rolls needed.</param>
    /// <returns>Modified rolls and history of changes.</returns>
    ModifierResult Apply(IReadOnlyList<int> rolls, int diceSides, Random random);
}
