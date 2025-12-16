using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class GameBoard(ISpatialModel spatialModel)
{
    // ─────────────
    // QUERIES
    // ─────────────

    public bool IsValid(Position position)
        => spatialModel.IsValidPosition(position);

    public IEnumerable<Position> GetNeighbors(Position position)
        => spatialModel.GetNeighbors(position);

    public int Distance(Position from, Position to)
        => spatialModel.Distance(from, to);

    // ─────────────
    // MOVEMENT
    // ─────────────

    public bool CanMoveActor(Actor actor, Position target)
        => spatialModel.CanMove(actor, target);

}
