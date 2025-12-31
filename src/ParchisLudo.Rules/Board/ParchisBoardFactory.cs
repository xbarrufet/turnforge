using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules.Board;

/// <summary>
/// Factory to create a GameBoard configured for Parchís.
/// Uses TurnForge's spatial model with TileId.
/// </summary>
public static class ParchisBoardFactory
{
    // ...
    
    /// <summary>
    /// Create a Board Descriptor for Parchís.
    /// </summary>
    public static DiscreteSpatialDescriptor CreateDescriptor()
    {
        // ... (connection logic remains) ...
        // Create all connections
        var connections = new List<(TileId, TileId)>();
        
        // Main circuit: 0-67 connected sequentially
        for (int i = 1; i < ParchisBoard.MainCircuitSize-1; i++)
        {
            connections.Add((new TileId($"track_{i}"), new TileId($"track_{i + 1}")));
            if(i>1)
            {
                connections.Add((new TileId($"track_{i}"), new TileId($"track_{i - 1}")));
            }
        }
        // Close the circuit
        connections.Add((new TileId($"track_{ParchisBoard.MainCircuitSize}"), new TileId("track_1")));
        connections.Add((new TileId($"track_{ParchisBoard.MainCircuitSize}"), new TileId($"track_{ParchisBoard.MainCircuitSize-1}")));
        connections.Add((new TileId("track_1"), new TileId($"track_{ParchisBoard.MainCircuitSize}")));
        
        // Finish lanes for Yellow
        AddFinishLaneConnections(connections, "yellow", ParchisBoard.YellowFinishEntry);
        // Finish lanes for Blue  
        AddFinishLaneConnections(connections, "blue", ParchisBoard.BlueFinishEntry);
        // Finish lanes for Red
        AddFinishLaneConnections(connections, "red", ParchisBoard.RedFinishEntry);
        // Finish lanes for Green
        AddFinishLaneConnections(connections, "green", ParchisBoard.GreenFinishEntry);

        AddSpawnAreas(connections);
        
        // Create descriptor
        var connectionDescriptors = connections.Select(c => 
            new DiscreteConnectionDescriptor(ConnectionId.New(), c.Item1, c.Item2))
            .ToList();

        var nodes = connections.SelectMany(c => new[] { c.Item1, c.Item2 })
            .Distinct()
            .ToList();
        
        return new DiscreteSpatialDescriptor(nodes, connectionDescriptors);
    }
    
    private static void AddFinishLaneConnections(List<(TileId, TileId)> connections, string color, string entryTrackPosition)
    {
        // Connect from track to first finish lane tile
        connections.Add((new TileId($"track_{entryTrackPosition}"),new TileId($"{color}_finish_1")));
        
        // Connect finish lane tiles sequentially
        for (int i = 1; i < ParchisBoard.FinishLaneSize - 1; i++)
        {
            connections.Add((new TileId($"{color}_finish_{i}"), new TileId($"{color}_finish_{i + 1}")));
            if(i>1)
            {
                connections.Add((new TileId($"{color}_finish_{i}"), new TileId($"{color}_finish_{i - 1}")));
            }
            // add center connection
            connections.Add((new TileId($"{color}_finish_{i}"), new TileId($"center_{color}")));
        }
        
        // Connect last finish lane to center
        connections.Add((new TileId($"{color}_finish_{ParchisBoard.FinishLaneSize - 1}"), new TileId("center")));
    }

    private static void AddSpawnAreas(List<(TileId, TileId)> connections)
    {
        connections.Add((new TileId("spawn_red"), new TileId(ParchisBoard.RedEntry)));
        connections.Add((new TileId("spawn_blue"), new TileId(ParchisBoard.BlueEntry)));
        connections.Add((new TileId("spawn_green"), new TileId(ParchisBoard.GreenEntry)));
        connections.Add((new TileId("spawn_yellow"), new TileId(ParchisBoard.YellowEntry)));
        
    }


}
