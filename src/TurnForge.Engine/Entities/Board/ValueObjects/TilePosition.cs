using System.Diagnostics;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.ValueObjects;

[DebuggerDisplay("Tile: {tileId.Value}")]
public readonly record struct TilePosition(TileId tileId) : IBoardPosition
{
    public TileId TileId => tileId;
    public BoardPositionKind Kind => BoardPositionKind.Tile;
    public IBoardPositionId Id { get => tileId; }

    public override string ToString() => $"TilePosition({TileId})";

    public static TilePosition FromTileId(TileId tileId) => new(tileId);
    public static TilePosition Empty => new(TileId.Empty);
}