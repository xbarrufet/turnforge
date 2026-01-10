namespace TurnForge.Engine.ValueObjects;

/// <summary>
/// Represents a unique identifier for a board position (Tile or Connection).
/// Provides a common interface for position-based operations and collection usage.
/// </summary>
public interface IBoardPositionId : IEquatable<IBoardPositionId>
{
    /// <summary>
    /// Gets the string representation of the ID.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Determines whether this position ID is empty.
    /// </summary>
    /// <returns>True if the ID is empty; otherwise, false.</returns>
    bool IsEmpty();

    /// <summary>
    /// Returns the string representation of the ID.
    /// </summary>
    string ToString();
    
    bool IsLimbo() => this is LimboPositionId;
    
    public static IBoardPositionId Limbo  = new LimboPositionId();
}

public class LimboPositionId : IBoardPositionId
{
    public string Value => "__Limbo__";

    public bool IsEmpty() => false;

    public override string ToString() => Value;

    public bool Equals(IBoardPositionId? other)
        => other is LimboPositionId;
}
