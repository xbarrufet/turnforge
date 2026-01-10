using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

public interface IGameBoard
{


    /// <summary>
    /// Tipo de board (discrete, continuous, hybrid…).
    /// </summary>
    TopologyKind Kind { get; }

    /// <summary>
    /// Topología del board (reglas de conexión y traversal).
    /// </summary>
    IReadOnlyDictionary<ZoneId, Zone> Zones { get; }
    IReadOnlyList<Connection> Connections { get; }
    
    Zone GetZoneByPosition(IBoardPosition position);
    bool IsValidPosition(IBoardPosition position);
    IEnumerable<Connection> GetConnectionsFrom(ZoneId zoneId);
    IEnumerable<Connection> GetConnectionsTo(ZoneId zoneId);
    (IEnumerable<Connection>, IEnumerable<Connection>) GetConnections(IEnumerable<ZoneId> zoneIds);
    
    
    IGameBoard Clone();
    
    
}


