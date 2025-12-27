using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Spatial;

public sealed class ConnectedGraphSpatialModel : ISpatialModel
{
    private readonly MutableTileGraph _tileGraph;

    public ConnectedGraphSpatialModel(MutableTileGraph tileGraph)
    {
        _tileGraph = tileGraph;
    }

    public bool IsValidPosition(Position position)
    {
        return _tileGraph.Exists(position.TileId);
    }

    public IEnumerable<Position> GetNeighbors(Position position)
    {
        var neighbors = _tileGraph.GetNeighbors(position.TileId);
        return neighbors.Select(tileId => new Position(tileId));
    }

    public bool CanMove(Actor actor, Position target)
    {
        if (actor.GetComponent<IPositionComponent>()?.IsDiscrete != true) return false;
        return _tileGraph.AreAdjacent(actor.GetComponent<IPositionComponent>()!.CurrentPosition.TileId, target.TileId);
    }

    public int Distance(Position from, Position to)
        => _tileGraph.ShortestPathLength(from.TileId, to.TileId);

    public void EnableConnection(Position from, Position to)
    {
        _tileGraph.EnableEdge(from.TileId, to.TileId);
    }

    public void DisableConnection(Position from, Position to)
    {
        _tileGraph.DisableEdge(from.TileId, to.TileId);
    }
}