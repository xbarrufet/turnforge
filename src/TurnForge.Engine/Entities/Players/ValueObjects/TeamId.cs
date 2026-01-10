namespace TurnForge.Engine.Entities.Players.ValueObjects;

public readonly record struct TeamId(string Value)
{
    public static TeamId From(string value) => new(value);
    
    public override string ToString() => Value;
    
    public static implicit operator string(TeamId id) => id.Value;
    
    public static implicit operator TeamId(string value) => new(value);
    
    public static readonly TeamId Empty = new(string.Empty);
    public static TeamId New() => new(Guid.NewGuid().ToString());
    public bool IsEmpty() => string.IsNullOrEmpty(Value);
    public bool IsNotEmpty() => !IsEmpty();
}