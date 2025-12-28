using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Orchestrator;

namespace TurnForge.Engine.Decisions.Entity.Interfaces;

public interface IDecision
{
    DecisionTiming Timing { get; }
    string OriginId { get; }

    /// <summary>
    /// Applies the decision to the given state, returning a modified state.
    /// This method preserves the immutability of the original state.
    /// </summary>
    /// <param name="state">The base game state.</param>
    /// <returns>The new game state with the decision applied.</returns>
    GameState Apply(GameState state);
}