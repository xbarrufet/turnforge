using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class Connection
{
    public ConnectionId Id { get; }
    public TileId FromAreaId { get; init; }
    public TileId ToAreaId { get; init; }
    public bool IsOpen { get; set; }

    public Connection(ConnectionId id, TileId fromAreaId, TileId toAreaId, bool isOpen = true)
    {
        Id = id;
        FromAreaId = fromAreaId;
        ToAreaId = toAreaId;
        IsOpen = isOpen;
    }
}