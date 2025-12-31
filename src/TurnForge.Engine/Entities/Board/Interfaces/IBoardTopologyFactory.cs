using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IBoardTopologyFactory
{
    IDiscreteTopology CreateDiscreteTopology(IReadOnlyList<(TileId positionFrom, TileId positionTo)> edges);

    // IBoardTopology CreateContinuosTopology();
}