using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;


public sealed class Zone
{
    public ZoneId Id { get; }
    public IZoneBound Bound { get; }
    public IReadOnlyList<IZoneBehaviour> Behaviours { get; }

    public Zone(
        ZoneId id,
        IZoneBound bound,
        IReadOnlyList<IZoneBehaviour> Behaviours)
    {
        Id = id;
        Bound = bound;
        Behaviours = Behaviours;
    }

    public bool Contains(Position position)
        => Bound.Contains(position);
}
