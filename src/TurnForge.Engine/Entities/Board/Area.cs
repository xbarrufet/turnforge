using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board;

public sealed class Area
{
    public TileId Id { get; }
    public Rectangle Bound { get; }

    private readonly HashSet<IAreaTrait> _Traits = [];

    public Area(TileId areaId, Rectangle bound)
    {
        this.Id = areaId;
        this.Bound = bound;
    }

    public Area(TileId areaId, Point topLeft, Point bottomRight)
    {
        this.Id = areaId;
        this.Bound = new Rectangle(topLeft, bottomRight);
    }

    public Area(TileId areaId, Point topLeft, int width, int height)
    {
        this.Id = areaId;
        this.Bound = new Rectangle(topLeft, width, height);
    }
    public override string ToString()
        => $"Area {Id} [Bound: {Bound}]";

    public void AddTrait(IAreaTrait trait)
        => _Traits.Add(trait);

    public bool HasTrait<T>() where T : IAreaTrait
        => _Traits.Any(t => t is T);

    public T? GetTrait<T>() where T : class, IAreaTrait
        => _Traits.OfType<T>().FirstOrDefault();

    public bool Contains(Vector position)
        => Bound.Contains(position);
}