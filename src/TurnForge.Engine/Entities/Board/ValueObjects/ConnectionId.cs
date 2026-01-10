namespace TurnForge.Engine.ValueObjects;

public readonly record struct ConnectionId(Guid Id) : IBoardPositionId
{
    public static ConnectionId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Id.ToString();

    public bool IsEmpty()
        => Id == Guid.Empty;

    public static ConnectionId Empty => new(Guid.Empty);

    // IBoardPositionId implementation
    string IBoardPositionId.Value => Id.ToString();

    bool IEquatable<IBoardPositionId>.Equals(IBoardPositionId? other)
        => other is ConnectionId connectionId && Equals(connectionId);
}