namespace TurnForge.Engine.ValueObjects;


public readonly record struct NodeId(string Value)
{
    public static NodeId New()
        => new(Guid.NewGuid().ToString());

    public override string ToString()
        => Value.ToString();
}