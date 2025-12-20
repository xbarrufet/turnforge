namespace TurnForge.Engine.ValueObjects;

public readonly record struct TileId(Guid Value)
{
    public static TileId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();

    public static TileId Empty { get; } = new(Guid.Empty);
    public bool IsEmpty() => this.Equals(Empty);

}