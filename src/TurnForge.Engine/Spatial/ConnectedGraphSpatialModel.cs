using TurnForge.Engine.Entities.Actors;
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
        throw new NotImplementedException();
    }

    public IEnumerable<Position> GetNeighbors(Position position)
    {
        return _tileGraph.GetNeighbors(position);
    }

    public bool CanMove(Actor actor, Position target)
    {
        return _tileGraph.AreAdjacent(actor.Position, target);
    }

    public int Distance(Position from, Position to)
        => _tileGraph.ShortestPathLength(from, to);

    public void EnableConnection(Position from, Position to)
    {
        _tileGraph.EnableEdge(from, to);
    }

    public void DisableConnection(Position from, Position to)
    {
        _tileGraph.DisableEdge(from, to);
    }
}