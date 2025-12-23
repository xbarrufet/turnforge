namespace TurnForge.Engine.ValueObjects;

public readonly record struct ConnectionId(Guid Value)
{
    public static ConnectionId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();

    internal bool IsEmpty()
    {
        return Value == Guid.Empty;
    }

    public static ConnectionId Empty => new(Guid.Empty);
}