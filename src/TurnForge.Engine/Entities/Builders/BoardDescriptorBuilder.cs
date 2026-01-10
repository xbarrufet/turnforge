using TurnForge.Engine.Core.Exceptions;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Entities.Builders;

public class BoardDescriptorBuilder
{
    private TopologyKind _kind = TopologyKind.Discrete;
    private readonly List<ZoneDescriptor> _zones = new();
    private readonly List<ConnectionDescriptor> _connections = new();
    
    public BoardDescriptorBuilder WithKind(TopologyKind kind)
    {
        _kind = kind;
        return this;
    }

    public BoardDescriptorBuilder WithZone(ZoneId id, Action<ZoneDescriptorBuilder> configure)
    {
        var builder = new ZoneDescriptorBuilder(id);
        configure(builder);
        _zones.Add(builder.Build());
        return this;
    }
    
    public BoardDescriptorBuilder WithConnection(Action<ConnectionDescriptorBuilder> configure)
    {
        var builder = new ConnectionDescriptorBuilder();
        configure(builder);
        _connections.Add(builder.Build());
        return this;
    }
    

    public BoardDescriptor Build()
    {
        
        _validate();
        return new BoardDescriptor(_kind, _zones, _connections);
    }

    private void _validate()
    {
        // Zones from and to connections must exist
        // and 
        var zoneIds = _zones.Select(z => z.ZoneId).ToHashSet();
        foreach (var connection in _connections)
        {
            if (!zoneIds.Contains(connection.From))
                throw new InvalidDescriptorException($"Connection 'From' zone '{connection.From}' does not exist");
            if (!zoneIds.Contains(connection.To))
                throw new InvalidDescriptorException($"Connection 'To' zone '{connection.To}' does not exist");
            var zoneFrom = _zones.Find(z => z.ZoneId == connection.From);
            var zoneTo = _zones.Find(z => z.ZoneId == connection.To);
            //validate that all connected positions are inside their zones
            foreach (var position in connection.ConnectionPosition.GetZoneFromConnectionPoint())
            {
                if (zoneFrom != null && !zoneFrom.ZoneTopology.IsInsideZone(position))
                    throw new InvalidDescriptorException(
                        $"Connection 'From' position '{position}' is not inside zone '{connection.From}'");
                foreach (var connectedPosition in connection.ConnectionPosition.GetZoneToConnectedPositionsByPositionId(
                             position))
                {
                    if (zoneTo != null && !zoneTo.ZoneTopology.IsInsideZone(connectedPosition))
                        throw new InvalidDescriptorException(
                            $"Connection 'To' position '{connectedPosition}' is not inside zone '{connection.To}'");
                }
            }
        }
    }

}