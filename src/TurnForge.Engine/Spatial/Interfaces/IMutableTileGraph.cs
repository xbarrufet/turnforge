using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Spatial.Interfaces;

public interface IMutableTileGraph: ITileGraph
{
    void EnableEdge(Position from, Position to);
    void DisableEdge(Position from, Position to);
}
