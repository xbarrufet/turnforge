using TurnForge.Engine.Definitions;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Core.State;

/// <summary>
/// Service responsible for calculating the projected state of the game
/// by applying a sequence of pending decisions onto a base state.
/// </summary>
public interface IStateProjector
{
    /// <summary>
    /// Projects a future state by applying decisions to the base state.
    /// Does not modify the base state.
    /// </summary>
    /// <param name="baseState">The starting immutable state.</param>
    /// <param name="decisions">The ordered list of decisions to apply.</param>
    /// <returns>A new GameState instance reflecting the changes.</returns>
    GameState Project(GameState baseState, IEnumerable<IDecision> decisions);
}
