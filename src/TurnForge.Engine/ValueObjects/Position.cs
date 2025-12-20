namespace TurnForge.Engine.ValueObjects;

public readonly struct Position
{

    private readonly Vector _vector;
    private readonly TileId _tileId;

    public Position()
    {
        _vector = Vector.Empty;
        _tileId = TileId.Empty;
    }

    public Position(TileId tileId)
    {
        _vector = Vector.Empty;
        _tileId = tileId;
    }

    public Position(Vector vector)
    {
        _vector = vector;
        _tileId = TileId.Empty;
    }


    public int X => IsContinuous ? _vector.X : throw new InvalidOperationException("Position has no value");
    public int Y => IsContinuous ? _vector.Y : throw new InvalidOperationException("Position has no value");
    public TileId TileId => IsDiscrete ? _tileId : throw new InvalidOperationException("Position has no tile");
    public bool IsDiscrete => _vector.IsEmpty() && !_tileId.IsEmpty();
    public bool IsContinuous => !_vector.IsEmpty() && _tileId.IsEmpty();
    public bool IsEmpty() => _vector.IsEmpty() && _tileId.IsEmpty();

    public static Position FromTile(TileId tile) => new Position(tile);
    public static Position FromWorld(Vector world) => new Position(world);
    public static Position Empty => new Position();

    public override string ToString()
    {
        if (IsDiscrete) return $"Tile:{_tileId}";
        if (IsContinuous) return $"Vector:{_vector}";
        return "Empty";
    }

    public bool Equals(Position other) => _vector.Equals(other._vector) && _tileId.Equals(other._tileId);
    public override bool Equals(object? obj) => obj is Position other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_vector, _tileId);
    public static bool operator ==(Position left, Position right) => left.Equals(right);
    public static bool operator !=(Position left, Position right) => !left.Equals(right);
}