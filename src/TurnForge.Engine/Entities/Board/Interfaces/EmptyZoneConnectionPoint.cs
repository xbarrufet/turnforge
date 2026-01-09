using TurnForge.Engine.Entities.Board.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public class EmptyZonConnectionPoint:IZoneConnectionPoint
{
    public bool InConnectionPoint(IBoardPosition position, out ZoneId zoneId)
    {
        throw new NotImplementedException();
    }
}