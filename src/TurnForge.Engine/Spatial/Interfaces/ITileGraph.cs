using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Spatial.Interfaces;

public interface ITileGraph
{
    bool AreAdjacent(Position from, Position to);

    IEnumerable<Position> GetNeighbors(Position tile);

    int ShortestPathLength(Position from, Position to);

    bool Exists(Position tile);
}