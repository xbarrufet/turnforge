using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Discrete;

public class DiscreteZoneConnectionPoint(
    ConnectedZones connectedZones,
    Dictionary<IBoardPositionId, IEnumerable<IBoardPositionId>> positions)
    : IZoneConnectionPoint
{
    public ZoneId From => connectedZones.From; 
    public ZoneId To => connectedZones.To; 
    
    public bool InConnectionPoint(IBoardPositionId position)
    {
        return positions.ContainsKey(position);
    }

    public IEnumerable<IBoardPositionId> GetConnectedPositionsId(IBoardPositionId position)
    {
        if (positions.TryGetValue(position, out var connectedPositions))
        {
            return connectedPositions;
        }
        return Enumerable.Empty<IBoardPositionId>();
    }
}