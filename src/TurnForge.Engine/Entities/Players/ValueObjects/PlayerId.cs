namespace TurnForge.Engine.Entities.Players.ValueObjects;

/// <summary>
/// Custom player identifier defined by the user.
/// Used to link agents to their controlling player before runtime EntityIds are assigned.
/// </summary>
public readonly record struct PlayerId(string Value)
{
    public static PlayerId From(string value) => new(value);
    
    public override string ToString() => Value;
    
    public static implicit operator string(PlayerId id) => id.Value;
    
    public static implicit operator PlayerId(string value) => new(value);
    
    public static readonly PlayerId Empty = new(string.Empty);
    public static PlayerId New() => new(Guid.NewGuid().ToString());
    public bool Equals(PlayerId other) => Value == other.Value;
    public bool IsEmpty() => string.IsNullOrEmpty(Value);
    public bool IsNotEmpty() => !IsEmpty();
}
