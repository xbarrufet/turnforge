using TurnForge.Engine.Decisions.Board;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Effects.Board;

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

        // 2. Create effect with metadata
        var effect = new BoardInitializedEffect(
            zoneCount: decision.Board.Zones.Count,
            spatialModelType: "Grid"  // Could extract from spatial model if available
        );

        // 3. Return updated state and effects
        return new ApplierResponse(newState, new[] { effect });
    }
}
