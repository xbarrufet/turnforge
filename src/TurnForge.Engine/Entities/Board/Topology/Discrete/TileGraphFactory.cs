using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Discrete;

public class TileGraphFactory
{
    
    public static TileGraph CreateEmptyTileGraph()
    {
        var tileGraph = new TileGraph();
        return tileGraph;
    }
    
    public static TileGraph CreateDefaultTileGraph(IEnumerable<(TileId A, TileId B)> connections)
    {
        var tileGraph = new TileGraph(connections);
        return tileGraph;
    }
    
    public static TileGraph CreateSingleTileGraph(TileId tileId)
    {
        var node = new List<(TileId, TileId)> { (tileId, tileId) };
        var singleTileGraph = new TileGraph(node);
        return singleTileGraph;
    }
    
    public static TileGraph CreateTrackTileGraph(IReadOnlyList<TileId> tileIds,bool circular=false, bool bidirectional=false)
    {
        var connections = new List<(TileId, TileId)>();
        TileId? previousTileId = null;
        
        foreach (var tileId in tileIds)
        {
            if (previousTileId != null)
            {
                connections.Add((previousTileId.Value, tileId));
            }

            if (bidirectional)
            {
                if (previousTileId != null)
                {
                    connections.Add((tileId, previousTileId.Value));
                }
            }
            previousTileId = tileId;
        }
        if (circular && tileIds.Any())
        {
            connections.Add((previousTileId!.Value, tileIds[0]));
            if (bidirectional)
            {
                connections.Add((tileIds[0], previousTileId!.Value));
            }
        }
        
        var trackTileGraph = new TileGraph(connections);
        return trackTileGraph;
    }

    /// <summary>
    /// Creates a linear track TileGraph with customizable ID pattern.
    /// </summary>
    /// <param name="numNodes">Number of nodes in the track</param>
    /// <param name="pattern">Format pattern for tile IDs. Use {0}=index
    /// Examples:
    ///   - "tile_{0}" -> "tile_0", "tile_1", etc.
    ///   - "node{0}" -> "node0", "node1", etc.
    ///   - "pos_{0:D2}" -> "pos_00", "pos_01", etc.
    /// </param>
    /// <param name="circular">Whether the track connects back to the first node</param>
    /// <returns>TileGraph with linear track connections</returns>
    public static TileGraph CreateTrackTileGraph(int numNodes, string pattern = "tile_{0}", bool circular = false)
    {
        var tileIds = new List<TileId>();
        for (int i = 0; i < numNodes; i++)
        {
            tileIds.Add(new TileId(string.Format(pattern, i)));
        }
        return CreateTrackTileGraph(tileIds, circular);
    }

    /// <summary>
    /// Creates a grid TileGraph with customizable ID pattern.
    /// </summary>
    /// <param name="rows">Number of rows in the grid</param>
    /// <param name="columns">Number of columns in the grid</param>
    /// <param name="pattern">Format pattern for tile IDs. Use {0}=prefix, {1}=row, {2}=column
    /// Examples:
    ///   - "{0}R{1}C{2}" -> "R0C0", "R0C1", etc.
    ///   - "tile_{1}_{2}" -> "tile_0_0", "tile_0_1", etc.
    ///   - "{1},{2}" -> "0,0", "0,1", etc.
    ///   - "pos_{1}x{2}" -> "pos_0x0", "pos_0x1", etc.
    /// </param>
    /// <returns>TileGraph with grid connections</returns>
    public static TileGraph CreateGridTileGraph(int rows, int columns, string pattern = "R{1}C{2}")
    {
        var connections = new List<(TileId, TileId)>();
        
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var currentTileId = new TileId(string.Format(pattern, "", r, c));
                
                // Connect to right neighbor
                if (c < columns - 1)
                {
                    var rightTileId = new TileId(string.Format(pattern, "", r, c + 1));
                    connections.Add((currentTileId, rightTileId));
                }
                
                // Connect to bottom neighbor
                if (r < rows - 1)
                {
                    var bottomTileId = new TileId(string.Format(pattern, "", r + 1, c));
                    connections.Add((currentTileId, bottomTileId));
                }
            }
        }
        
        var gridTileGraph = new TileGraph(connections);
        return gridTileGraph;
    }
    
}