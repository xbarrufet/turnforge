using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class Area
{
    public AreaId Id { get; }
    public Rectangle Bound { get; }
    
    private readonly HashSet<IAreaBehaviour> _Behaviours = [];
    
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
    
    public void AddBehaviour(IAreaBehaviour Behaviour)
        => _Behaviours.Add(Behaviour);

    public bool HasBehaviour<T>() where T : IAreaBehaviour
        => _Behaviours.Any(t => t is T);

    public T? GetBehaviour<T>() where T : class, IAreaBehaviour
        => _Behaviours.OfType<T>().FirstOrDefault();
    
    public bool Contains(Position position)
        => Bound.Contains(position);
}