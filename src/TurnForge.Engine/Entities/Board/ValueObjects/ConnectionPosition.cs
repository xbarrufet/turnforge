using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.ValueObjects;

public readonly record struct ConnectionPosition(ConnectionId connectionId) : IBoardPosition
{
    public ConnectionId ConnectionId => connectionId;
    public BoardPositionKind Kind => BoardPositionKind.Connection;

    public override string ToString() => $"ConnectionPosition({ConnectionId})";

    public static ConnectionPosition FromConnectionId(ConnectionId connectionId) => new(connectionId);
    public static ConnectionPosition Empty => new(ConnectionId.Empty);
}