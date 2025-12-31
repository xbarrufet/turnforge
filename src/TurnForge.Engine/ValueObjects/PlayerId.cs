namespace TurnForge.Engine.ValueObjects;

/// <summary>
/// Custom player identifier defined by the user.
/// Used to link agents to their controlling player before runtime EntityIds are assigned.
/// </summary>
public readonly record struct PlayerId(string Value)
{
    public static PlayerId From(string value) => new(value);
    
    public override string ToString() => Value;
    
    public static implicit operator string(PlayerId id) => id.Value;
}
