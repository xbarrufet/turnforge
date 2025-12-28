namespace TurnForge.Engine.ValueObjects;

public readonly record struct ReactionId(string Value)
{
    public static ReactionId New() => new(Guid.NewGuid().ToString());
};