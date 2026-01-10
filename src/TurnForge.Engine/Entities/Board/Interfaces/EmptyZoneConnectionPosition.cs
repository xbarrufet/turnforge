using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public class EmptyZoneConnectionPoint:IZoneConnectionPoint
{
    public bool InConnectionPoint(IBoardPosition position, out ZoneId zoneId)
    {
        throw new NotImplementedException();
    }

    public ZoneId From { get; }
    public ZoneId To { get; }
    public bool InConnectionPoint(IBoardPositionId position)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IBoardPositionId> GetConnectedPositionsId(IBoardPositionId position)
    {
        throw new NotImplementedException();
    }
}