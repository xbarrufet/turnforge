using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Components;

public class GameBoardComponent(ISpatialModel spatialModel) : IGameEntityComponent
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
