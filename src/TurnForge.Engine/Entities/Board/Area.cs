using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class Area
{
    public AreaId Id { get; }
    public Rectangle Bound { get; }
    
    private readonly HashSet<IAreaTrait> _traits = [];
    
    public Area(AreaId areaId, Rectangle bound)
    {
        this.Id = areaId;
        this.Bound = bound;
    }
    
    public Area(AreaId areaId, Point topLeft, Point bottomRight)
    {
        this.Id = areaId;
        this.Bound = new Rectangle(topLeft, bottomRight);
    }

    public Area(AreaId areaId, Point topLeft,  int width, int height)
    {
        this.Id = areaId;
        this.Bound = new Rectangle(topLeft, width, height);
    }
    public override string ToString()
        => $"Area {Id} [Bound: {Bound}]";
    
    public void AddTrait(IAreaTrait trait)
        => _traits.Add(trait);

    public bool HasTrait<T>() where T : IAreaTrait
        => _traits.Any(t => t is T);

    public T? GetTrait<T>() where T : class, IAreaTrait
        => _traits.OfType<T>().FirstOrDefault();
    
    public bool Contains(Position position)
        => Bound.Contains(position);
}