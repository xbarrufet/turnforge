namespace TurnForge.Engine.ValueObjects;

public readonly struct Position
{

    private readonly Vector _vector;
    private readonly TileId _tileId;
    private readonly ConnectionId _connectionId;
    private readonly TileId[] _area;


    public Position()
    {
        _vector = Vector.Empty;
        _tileId = TileId.Empty;
        _connectionId = ConnectionId.Empty;
        _area = [];
    }

    public Position(TileId tileId)
    {
        _vector = Vector.Empty;
        _tileId = tileId;
        _connectionId = ConnectionId.Empty;
        _area = [];
    }

    public Position(Vector vector)
    {
        _vector = vector;
        _tileId = TileId.Empty;
        _connectionId = ConnectionId.Empty;
        _area = [];
    }

     public Position(ConnectionId connectionId)
    {
        _connectionId = connectionId;
        _area = [];
        _vector = Vector.Empty;
        _tileId = TileId.Empty;
    }

    public Position(TileId[] area)
    {
        _area = area ?? throw new ArgumentNullException(nameof(area));
        _connectionId = ConnectionId.Empty;
        _vector = Vector.Empty;
        _tileId = TileId.Empty;
    }


    public int X => IsVector ? _vector.X : throw new InvalidOperationException("Position has no value");
    public int Y => IsVector ? _vector.Y : throw new InvalidOperationException("Position has no value");
    public TileId TileId => IsTile ? _tileId : throw new InvalidOperationException("Position has no tile");
    public ConnectionId ConnectionId => IsConnection ? _connectionId : throw new InvalidOperationException("Position has no connection");
    public TileId[] Area => IsArea ? _area : throw new InvalidOperationException("Position has no area");
    
    public bool IsTile => !_tileId.IsEmpty() && _connectionId.IsEmpty() && _area.Length == 0;
    public bool IsVector => !_vector.IsEmpty() && _tileId.IsEmpty() && _connectionId.IsEmpty() && _area.Length == 0;
    public bool IsConnection => !_connectionId.IsEmpty() && _tileId.IsEmpty() && _area.Length == 0;
    public bool IsArea => _area.Length > 0 && _tileId.IsEmpty() && _connectionId.IsEmpty() && _vector.IsEmpty();


    public bool IsEmpty() => _vector.IsEmpty() && _tileId.IsEmpty() && _connectionId.IsEmpty() && _area.Length == 0;



    public static Position FromTile(TileId tile) => new(tile);
    public static Position FromConnection(ConnectionId connectionId) => new(connectionId);
    public static Position FromWorld(Vector world) => new(world);
    public static Position Empty => new();

    public override string ToString()
    {
        if (IsTile) return $"Tile:{_tileId}";
        if (IsVector) return $"Vector:{_vector}";
        if (IsConnection) return $"Connection:{_connectionId}";
        if (IsArea) return $"Area:{_area}";
        return "Empty";
    }

    public bool Equals(Position other) => _vector.Equals(other._vector) && _tileId.Equals(other._tileId);
    public override bool Equals(object? obj) => obj is Position other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_vector, _tileId);
    public static bool operator ==(Position left, Position right) => left.Equals(right);
    public static bool operator !=(Position left, Position right) => !left.Equals(right);
}