using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Decisions.Board;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Events;

namespace TurnForge.Engine.Appliers.Board;

/// <summary>
/// Applier that adds the initialized board to game state.
/// Generates BoardInitializedEffect for UI/logging.
/// </summary>
public sealed class InitializeBoardApplier : IApplier<InitializeBoardDecision>
{
    public ApplierResponse Apply(InitializeBoardDecision decision, GameState state)
    {
        // 1. Add board to state (immutable update)
        var newState = state.WithBoard(decision.Board);

        // 2. Create event with metadata
        var gameEvent = new BoardInitializedEvent(
            zoneCount: decision.Board.Zones.Count,
            spatialModelType: "Grid"  // Could extract from spatial model if available
        );

        // 3. Return updated state and events
        return new ApplierResponse(newState, new[] { gameEvent });
    }
}
