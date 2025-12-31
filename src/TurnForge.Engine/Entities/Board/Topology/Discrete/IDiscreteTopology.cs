using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Discrete;

public interface IDiscreteTopology : IBoardTopology
{
    IEnumerable<TileId> GetAdjacents(TilePosition tile);
    int GetDistance(TilePosition start, TilePosition end);
    bool IsConnected(TilePosition a, TilePosition b);
}