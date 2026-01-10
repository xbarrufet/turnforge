using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IZoneConnectionPosition
{
    
    bool InConnectionPoint(IBoardPositionId position);
    IEnumerable<IBoardPositionId> GetZoneToConnectedPositionsByPositionId(IBoardPositionId position);
    IEnumerable<IBoardPositionId> GetZoneFromConnectionPoint();
    public static IZoneConnectionPosition Empty => new EmptyZoneConnectionPosition();
    public int NumberOfConnections { get; }
}