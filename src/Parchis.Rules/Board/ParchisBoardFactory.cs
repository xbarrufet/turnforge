using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules.Board;

/// <summary>
/// Factory to create a GameBoard configured for Parchís.
/// Uses TurnForge's spatial model with TileId.
/// </summary>
public static class ParchisBoardFactory
{
    // Pre-generated stable GUIDs for each tile (for consistency)
    private static readonly Dictionary<string, TileId> _tileMap = new();
    
    /// <summary>
    /// Create a GameBoard for Parchís.
    /// </summary>
    public static GameBoard Create()
    {
        // Create all connections
        var connections = new List<(TileId, TileId)>();
        
        // Main circuit: 0-67 connected sequentially
        for (int i = 0; i < ParchisBoard.MainCircuitSize - 1; i++)
        {
            connections.Add((GetTileId($"track_{i}"), GetTileId($"track_{i + 1}")));
        }
        // Close the circuit
        connections.Add((GetTileId($"track_{ParchisBoard.MainCircuitSize - 1}"), GetTileId("track_0")));
        
        // Finish lanes for Yellow
        AddFinishLaneConnections(connections, "yellow", ParchisBoard.YellowFinishEntry);
        
        // Finish lanes for Blue  
        AddFinishLaneConnections(connections, "blue", ParchisBoard.BlueFinishEntry);
        
        // Create graph from connections
        var graph = new MutableTileGraph(connections);
        var spatialModel = new ConnectedGraphSpatialModel(graph);
        
        return new GameBoard(spatialModel);
    }
    
    private static void AddFinishLaneConnections(List<(TileId, TileId)> connections, string color, int entryTrackPosition)
    {
        // Connect from track to first finish lane tile
        connections.Add((GetTileId($"track_{entryTrackPosition}"), GetTileId($"{color}_finish_0")));
        
        // Connect finish lane tiles sequentially
        for (int i = 0; i < ParchisBoard.FinishLaneSize - 1; i++)
        {
            connections.Add((GetTileId($"{color}_finish_{i}"), GetTileId($"{color}_finish_{i + 1}")));
        }
        
        // Connect last finish lane to center
        connections.Add((GetTileId($"{color}_finish_{ParchisBoard.FinishLaneSize - 1}"), GetTileId("center")));
    }
    
    /// <summary>
    /// Get or create a stable TileId for a logical tile name.
    /// </summary>
    public static TileId GetTileId(string logicalName)
    {
        if (!_tileMap.TryGetValue(logicalName, out var tileId))
        {
            // Create deterministic GUID from name (for stability)
            tileId = new TileId(CreateDeterministicGuid(logicalName));
            _tileMap[logicalName] = tileId;
        }
        return tileId;
    }
    
    /// <summary>
    /// Get logical name for a TileId (reverse lookup).
    /// </summary>
    public static string? GetLogicalName(TileId tileId)
    {
        return _tileMap.FirstOrDefault(kvp => kvp.Value.Equals(tileId)).Key;
    }
    
    private static Guid CreateDeterministicGuid(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes($"parchis:{input}"));
        return new Guid(hash);
    }
}
