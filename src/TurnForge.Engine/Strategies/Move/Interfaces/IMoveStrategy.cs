using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Strategies.Move.Interfaces;

/// <summary>
/// Strategy to define how movement commands are processed.
/// Allows injecting custom logic like checking tile traits, triggering traps, etc.
/// </summary>
public interface IMoveStrategy
{
    /// <summary>
    /// Processes the move command and returns a list of resulting decisions.
    /// </summary>
    /// <param name="command">The move command to process.</param>
    /// <param name="state">The current game state (map, entities, etc.).</param>
    /// <returns>Decisions to apply (e.g., MoveDecision, DamageDecision, SpawnDecision).</returns>
    IEnumerable<IDecision> Process(MoveCommand command, GameState state);
}
