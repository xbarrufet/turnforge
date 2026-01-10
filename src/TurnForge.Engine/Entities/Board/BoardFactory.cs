using TurnForge.Engine.Definitions.Board;

using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class BoardFactory : IBoardFactory
{
    private GenericEntityFactory _genericEntityFactory;
    
    public BoardFactory(GenericEntityFactory genericEntityFactory)
    {
        _genericEntityFactory = genericEntityFactory;
    }   
    public IGameBoard CreateGameBoard(BoardDescriptor boardDescriptor)
    {
       var zones = BuildBoardZones(boardDescriptor.Zones);
       var connections = BuildBoardConnections(boardDescriptor.Connections);    
       var gameBoard = new GameBoard(boardDescriptor.Kind,zones, connections);
       return gameBoard;
    }
    
    public IReadOnlyDictionary<ZoneId,Zone> BuildBoardZones(IReadOnlyList<ZoneDescriptor> zoneDescriptors)
    {
        var zones = new Dictionary<ZoneId, Zone>();
        foreach (var zoneDescriptor in zoneDescriptors)
        {
            var zone = _genericEntityFactory.BuildZone(zoneDescriptor);
            zones[zone.ZoneId] = zone;
        }
        return zones;
    }
    
    public IReadOnlyList<Connection> BuildBoardConnections(IReadOnlyList<ConnectionDescriptor> connectionDescriptors)
    {
        var connections = new List<Connection>();
        foreach (var connectionDescriptor in connectionDescriptors)
        {
            var connection= _genericEntityFactory.BuildConnection(connectionDescriptor);
            connections.Add(connection);
        }
        return connections;
    }
    
}