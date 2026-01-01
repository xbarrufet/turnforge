namespace TurnForge.Engine.ValueObjects;

/// <summary>
/// Strongly typed identifier for a Action.
/// </summary>
public readonly record struct ActionId(string Value)
    {
        public override string ToString() => Value;
    }