using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.ValueObjects;

public record struct ZoneId(string Value) : IBoardPositionId
{
    public static ZoneId New()
        => new(Guid.NewGuid().ToString());

    public override string ToString()
        => Value;

    public static ZoneId Empty { get; } = new(string.Empty);

    public bool IsEmpty() => string.IsNullOrEmpty(Value);

    public static ZoneId From(Guid value) => new(value.ToString());

    // IBoardPositionId implementation
    public bool Equals(IBoardPositionId? other)
        => other is ZoneId zoneId && zoneId.Value == Value;
}