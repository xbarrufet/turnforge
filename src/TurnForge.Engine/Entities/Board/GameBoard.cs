using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;


/// <summary>
/// Implementación base de un board de juego.
/// Coordina Topology y SpatialIndex para responder queries espaciales.
/// </summary>
public sealed class GameBoard(
    TopologyKind kind,
    IReadOnlyDictionary<ZoneId, Zone> zones,
    IReadOnlyList<Connection> connections)
    : IGameBoard
{
    public TopologyKind Kind { get; } = kind;
    public IReadOnlyDictionary<ZoneId, Zone> Zones => zones;
    public IReadOnlyList<Connection> Connections => connections;

    public Zone GetZoneByPosition(IBoardPosition position)
    {
        if (position.IsLimbo())
        {
            throw new InvalidOperationException("Cannot get zone for Limbo position");
        }

        // Intentamos obtener el ZoneId desde el IBoardPositionId
        if (position.Id is ZoneId zoneId)
        {
            if (zones.TryGetValue(zoneId, out var zone))
            {
                return zone;
            }
        }

        throw new KeyNotFoundException($"Zone not found for position: {position.Id}");
    }

    public bool IsValidPosition(IBoardPosition position)
    {
        if (position.IsLimbo())
        {
            return true; // Limbo is always valid
        }

        if (position.Id is ZoneId zoneId)
        {
            return zones.ContainsKey(zoneId);
        }

        return false;
    }

    public IEnumerable<Connection> GetConnectionsFrom(ZoneId zoneId)
    {
        // TODO: Las conexiones deberían tener propiedades From/To para filtrar correctamente
        // Por ahora devolvemos todas las conexiones hasta que se implemente la estructura completa
        return connections.Where(c => IsConnectionFromZone(c, zoneId));
    }

    public IEnumerable<Connection> GetConnectionsTo(ZoneId zoneId)
    {
        // TODO: Las conexiones deberían tener propiedades From/To para filtrar correctamente
        return connections.Where(c => IsConnectionToZone(c, zoneId));
    }

    public (IEnumerable<Connection>, IEnumerable<Connection>) GetConnections(IEnumerable<ZoneId> zoneIds)
    {
        var zoneIdSet = zoneIds.ToHashSet();
        var fromConnections = connections.Where(c => zoneIdSet.Any(zId => IsConnectionFromZone(c, zId)));
        var toConnections = connections.Where(c => zoneIdSet.Any(zId => IsConnectionToZone(c, zId)));
        
        return (fromConnections, toConnections);
    }

    public IGameBoard Clone()
    {
        var clonedZones = new Dictionary<ZoneId, Zone>(zones);
        var clonedConnections = new List<Connection>(connections);
        
        return new GameBoard( Kind, clonedZones, clonedConnections);
    }

    private bool IsConnectionFromZone(Connection _, ZoneId __)
    {
        // TODO: Implementar cuando Connection tenga propiedades From/To
        // Por ahora retornamos false como placeholder
        return false;
    }

    private bool IsConnectionToZone(Connection _, ZoneId __)
    {
        // TODO: Implementar cuando Connection tenga propiedades From/To
        // Por ahora retornamos false como placeholder
        return false;
    }
}

