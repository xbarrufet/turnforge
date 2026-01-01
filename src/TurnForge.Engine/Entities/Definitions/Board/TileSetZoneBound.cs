using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Board;

public class TileSetZoneBound : IZoneBound
{
    public IReadOnlySet<TileId> Tiles { get; }

    public TileSetZoneBound(IEnumerable<TileId> tiles)
    {
        Tiles = tiles.ToHashSet();
    }

    public TileSetZoneBound(params TileId[] tiles)
    {
        Tiles = tiles.ToHashSet();
    }

    public bool Contains(IBoardPosition position)
    {
        if (position is TilePosition tilePosition)
        {
            return Tiles.Contains(tilePosition.TileId);
        }
        return false;
    }
}
