namespace TurnForge.Engine.ValueObjects;

public readonly record struct TileId(string Value) : IBoardPositionId
{
    public static TileId New()
        => new(Guid.NewGuid().ToString());

    public override string ToString()
        => Value;

    public static TileId Empty { get; } = new(string.Empty);

    public bool IsEmpty() => string.IsNullOrEmpty(Value);

    public static TileId From(Guid value) => new(value.ToString());

    // Explicit interface implementation
    bool IEquatable<IBoardPositionId>.Equals(IBoardPositionId? other)
        => other is TileId tileId && Equals(tileId);
}