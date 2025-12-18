using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class TileSetZoneBound : IZoneBound
{
    private readonly HashSet<Position> _tiles;

    public TileSetZoneBound(IEnumerable<Position> tiles)
    {
        _tiles = tiles.ToHashSet();
    }

    public bool Contains(Position pos)
        => _tiles.Contains(pos);
}
