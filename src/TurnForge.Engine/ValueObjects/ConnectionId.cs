namespace TurnForge.Engine.ValueObjects;

public readonly record struct ConnectionId(Guid Value)
{
    public static ConnectionId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}