using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public class EmptyZoneConnectionPosition:IZoneConnectionPosition
{
    
    public bool InConnectionPoint(IBoardPosition position)
    {
        throw new NotImplementedException();
    }

    public bool InConnectionPoint(IBoardPositionId position)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IBoardPositionId> GetZoneToConnectedPositionsByPositionId(IBoardPositionId position)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IBoardPositionId> GetZoneFromConnectionPoint()
    {
        throw new NotImplementedException();
    }

    public int NumberOfConnections => 0;
}