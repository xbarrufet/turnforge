using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.ValueObjects;

/// <summary>
/// Position on a connection (edge) between two tiles.
/// Used for ConnectionEntities that "live" on connections rather than tiles.
/// </summary>
public readonly record struct ConnectionPosition(
    TileId From, 
    TileId To
) : IBoardPosition
{
    public BoardPositionKind Kind => BoardPositionKind.Connection;

    /// <summary>
    /// Create a ConnectionPosition between two tiles.
    /// </summary>
    public static ConnectionPosition Between(TileId from, TileId to) 
        => new(from, to);
    
    /// <summary>
    /// Create a ConnectionPosition from string tile IDs.
    /// </summary>
    public static ConnectionPosition Between(string from, string to) 
        => new(new TileId(from), new TileId(to));
        
    public static ConnectionPosition Empty => new(TileId.Empty, TileId.Empty);
        
    public override string ToString() => $"{From}→{To}";
}