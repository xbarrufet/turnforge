using TurnForge.Engine.Core.Exceptions;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Discrete;

public class DiscreteZoneConnectionPosition(
    Dictionary<IBoardPositionId, IEnumerable<IBoardPositionId>> positions)
    : IZoneConnectionPosition
{
    
    public bool InConnectionPoint(IBoardPositionId position)
    {
        return positions.ContainsKey(position);
    }

    public IEnumerable<IBoardPositionId> GetZoneToConnectedPositionsByPositionId(IBoardPositionId position)
    {
        if (positions.TryGetValue(position, out var connectedPositions))
        {
            return connectedPositions;
        }
        return Enumerable.Empty<IBoardPositionId>();
    }

    public IEnumerable<IBoardPositionId> GetZoneFromConnectionPoint()
    {
        return positions.Keys;  
    }

    public int NumberOfConnections => positions.Count;
}

public class DiscreteZoneConnectionPositionBuilder
{
    private readonly Dictionary<IBoardPositionId, List<IBoardPositionId>> _positions = new();

    public DiscreteZoneConnectionPositionBuilder AddConnection(
        IBoardPositionId from,
        IBoardPositionId to)
    {
        if (!_positions.ContainsKey(from))
        {
            _positions[from] = new List<IBoardPositionId>();
        }
        _positions[from].Add(to);
        return this;
    }

    public DiscreteZoneConnectionPosition Build()
    {
        _validate();
        var finalizedPositions = _positions.ToDictionary(
            kvp => kvp.Key,
            kvp => (IEnumerable<IBoardPositionId>)kvp.Value);
        return new DiscreteZoneConnectionPosition(finalizedPositions);
    }

    private void _validate()
    {
        //at least one connection
        if (_positions.Count == 0)
            throw new InvalidDescriptorException("ZoneConnectionPosition must have at least one connection");
    }
}