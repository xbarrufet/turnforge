namespace TurnForge.Engine.ValueObjects;

public readonly record struct AreaId(Guid Value)
{
    public static AreaId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
    
}