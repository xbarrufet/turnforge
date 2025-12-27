using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board;

public static class ZoneBoundFactory
{
    public static IZoneBound Create(ZoneBoundType type, IEnumerable<Position> tiles)
    {
        return type switch
        {
            ZoneBoundType.TileSet =>
                new TileSetZoneBound(tiles),
            _ => throw new NotSupportedException()
        };
    }
}

public enum ZoneBoundType
{
    TileSet
}
