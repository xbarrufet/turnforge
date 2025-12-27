using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board;

public sealed class GameBoard(ISpatialModel spatialModel) : GameEntity(EntityId.New(), string.Empty, string.Empty, string.Empty)
{


    private readonly List<Zone> _zones = [];
    public IReadOnlyList<Zone> Zones => _zones.AsReadOnly();

    // ─────────────
    // SETUP
    // ─────────────
    public void AddZone(Zone zone)
    {
        _zones.Add(zone);
    }

    // ─────────────
    // QUERIES
    // ─────────────


    public bool IsValid(Position position)
        => spatialModel.IsValidPosition(position);

    public IEnumerable<Position> GetNeighbors(Position position)
        => spatialModel.GetNeighbors(position);

    public int Distance(Position from, Position to)
        => spatialModel.Distance(from, to);


    public IEnumerable<Zone> GetZonesAt(Position position)
        => _zones.Where(z => z.Contains(position));
    // ─────────────
    // MOVEMENT
    // ─────────────

    public bool CanMoveActor(Actor actor, Position target)
        => spatialModel.CanMove(actor, target);


}
