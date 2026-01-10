namespace TurnForge.Engine.ValueObjects;

public record struct Category(string Value)
{
    public override string ToString() => Value;
    public static Category Empty => new(string.Empty);
}
