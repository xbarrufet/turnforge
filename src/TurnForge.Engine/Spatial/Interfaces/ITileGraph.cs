using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Spatial.Interfaces;

public interface ITileGraph
{
    bool AreAdjacent(TileId from, TileId to);

    IEnumerable<TileId> GetNeighbors(TileId tile);

    int ShortestPathLength(TileId from, TileId to);

    bool Exists(TileId tile);
}