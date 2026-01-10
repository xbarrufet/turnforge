using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Discrete;

public interface IDiscreteZoneTopology : IZoneTopology
{
    IEnumerable<TileId> GetAdjacents(TileId tileid);
    bool IsConnected(TileId a, TileId b);
    
}