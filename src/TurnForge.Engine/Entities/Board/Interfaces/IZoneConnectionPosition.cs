using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IZoneConnectionPoint
{
    ZoneId From { get; }
    ZoneId To { get; }
    bool InConnectionPoint(IBoardPositionId position);
    IEnumerable<IBoardPositionId> GetConnectedPositionsId(IBoardPositionId position);
    public static IZoneConnectionPoint Empty => new EmptyZoneConnectionPoint();
}