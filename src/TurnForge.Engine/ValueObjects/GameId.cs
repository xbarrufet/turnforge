namespace TurnForge.Engine.ValueObjects;

public readonly record struct GameId(Guid Value)
{
    public static GameId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}