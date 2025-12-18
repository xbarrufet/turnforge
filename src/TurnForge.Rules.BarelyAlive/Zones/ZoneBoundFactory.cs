using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Rules.BarelyAlive.Dto;

namespace TurnForge.Rules.BarelyAlive.Zones;

public static class ZoneBoundFactory
{
    public static IZoneBound Create(ZoneBoundDto dto)
        => dto.Type switch
        {
            "Rect" => new RectZoneBound(
                dto.X!.Value,
                dto.Y!.Value,
                dto.Width!.Value,
                dto.Height!.Value
            ),

            "TileSet" => new TileSetZoneBound(
                dto.Tiles!
            ),

            _ => throw new NotSupportedException(
                $"ZoneBound '{dto.Type}' not supported")
        };
}
