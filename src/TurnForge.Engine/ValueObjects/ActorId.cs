namespace TurnForge.Engine.ValueObjects;


public readonly record struct ActorId(Guid Value)
{
    public static ActorId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}