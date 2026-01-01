using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities.Board.Definitions; // Correct namespace for DiscretBoardDefinition
using TurnForge.Engine.Entities.Board.Interfaces;

namespace Parchis.Rules.Board;

/// <summary>
/// Factory to create a GameBoard configured for Parchís.
/// Uses TurnForge's spatial model with TileId.
/// </summary>
public static class ParchisBoardFactory
{
    
    /// <summary>
    /// Create a Board Definition for Parchís.
    /// </summary>
    public static DiscretBoardDefinition CreateDescriptor(string id = "parchis_board")
    {
        var boardDef = new DiscretBoardDefinition(id);
        
        // Connections list for helper methods
        var connections = new List<(string From, string To)>();
        
        // Main circuit: 0-67 connected sequentially
        for (int i = 1; i < ParchisBoard.MainCircuitSize-1; i++)
        {
            connections.Add(($"track_{i}", $"track_{i + 1}"));
            if(i>1)
            {
                connections.Add(($"track_{i}", $"track_{i - 1}"));
            }
        }
        // Close the circuit
        connections.Add(($"track_{ParchisBoard.MainCircuitSize}", "track_1"));
        connections.Add(($"track_{ParchisBoard.MainCircuitSize}", $"track_{ParchisBoard.MainCircuitSize-1}"));
        connections.Add(("track_1", $"track_{ParchisBoard.MainCircuitSize}"));
        
        // Finish lanes for Yellow
        AddFinishLaneConnections(connections, "yellow", ParchisBoard.YellowFinishEntry);
        // Finish lanes for Blue  
        AddFinishLaneConnections(connections, "blue", ParchisBoard.BlueFinishEntry);
        // Finish lanes for Red
        AddFinishLaneConnections(connections, "red", ParchisBoard.RedFinishEntry);
        // Finish lanes for Green
        AddFinishLaneConnections(connections, "green", ParchisBoard.GreenFinishEntry);

        AddSpawnAreas(connections);
        
        // Add connections to board definition
        foreach(var conn in connections)
        {
            boardDef.AddTileFromStringConnection(conn.From, conn.To);
        }

        return boardDef;
    }
    
    private static void AddFinishLaneConnections(List<(string, string)> connections, string color, string entryTrackPosition)
    {
        // Connect from track to first finish lane tile
        connections.Add(($"track_{entryTrackPosition}", $"{color}_finish_1"));
        
        // Connect finish lane tiles sequentially
        for (int i = 1; i < ParchisBoard.FinishLaneSize - 1; i++)
        {
            connections.Add(($"{color}_finish_{i}", $"{color}_finish_{i + 1}"));
            if(i>1)
            {
                connections.Add(($"{color}_finish_{i}", $"{color}_finish_{i - 1}"));
            }
            // add center connection
            connections.Add(($"{color}_finish_{i}", $"center_{color}"));
        }
        
        // Connect last finish lane to center
        connections.Add(($"{color}_finish_{ParchisBoard.FinishLaneSize - 1}", "center"));
    }

    private static void AddSpawnAreas(List<(string, string)> connections)
    {
        connections.Add(("spawn_red", ParchisBoard.RedEntry));
        connections.Add(("spawn_blue", ParchisBoard.BlueEntry));
        connections.Add(("spawn_green", ParchisBoard.GreenEntry));
        connections.Add(("spawn_yellow", ParchisBoard.YellowEntry));
        
    }



}
