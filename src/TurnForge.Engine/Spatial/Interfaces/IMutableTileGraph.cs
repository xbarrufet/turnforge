using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Spatial.Interfaces;

public interface IMutableTileGraph : ITileGraph
{
    void EnableEdge(TileId from, TileId to);
    void DisableEdge(TileId from, TileId to);
}
