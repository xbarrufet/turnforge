using System.Linq;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class BoardTopologyFactory : IBoardTopologyFactory
{
    public IDiscreteTopology CreateDiscreteTopology(IReadOnlyList<(TileId positionFrom, TileId positionTo)> edges)
    {
        return new TileGraph(edges.Select(e => (e.positionFrom, e.positionTo)));
    }
}