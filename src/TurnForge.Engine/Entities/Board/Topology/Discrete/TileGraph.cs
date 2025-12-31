using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Topology.Discrete;

public sealed class TileGraph : IDiscreteTopology
{

    private readonly Dictionary<TileId, HashSet<TileId>> _adjacency;
    private readonly HashSet<TileId> _nodes;
    
    public TileGraph() {
        _adjacency = new();
        _nodes = new HashSet<TileId>();
    }

    public TileGraph(IEnumerable<(TileId A, TileId B)> connections)
    {
        _adjacency = new();
        _nodes = new HashSet<TileId>();
        
        foreach (var (a, b) in connections)
        {
            AddConnection(a, b);
            
            // Ensure nodes are tracked
            _nodes.Add(a);
            _nodes.Add(b);
        }
    }

    // IBoardTopology Implementation
    public bool IsValidPosition(IBoardPosition position)
    {
        return position is TilePosition tp && _nodes.Contains(tp.TileId);
    }

    public bool CanTraverse(IBoardPosition from, IBoardPosition to)
    {
        if (from is TilePosition tFrom && to is TilePosition tTo)
        {
            return IsConnected(tFrom, tTo);
        }
        return false;
    }

    public int Distance(IBoardPosition from, IBoardPosition to)
    {
        if (from is TilePosition tFrom && to is TilePosition tTo)
        {
            return GetDistance(tFrom, tTo);
        }
        return -1;
    }
   


   

    private void AddConnection(TileId a, TileId b)
    {
        if (!_adjacency.ContainsKey(a)) _adjacency[a] = new HashSet<TileId>();
        if (!_adjacency.ContainsKey(b)) _adjacency[b] = new HashSet<TileId>();
        
        _adjacency[a].Add(b);
        _adjacency[b].Add(a);
    }

    public IEnumerable<TileId> GetAdjacents(TilePosition tile)
    {
        if (_adjacency.TryGetValue(tile.TileId, out var neighbors))
        {
            return neighbors;
        }
        return Enumerable.Empty<TileId>();
    }

    public int GetDistance(TilePosition start, TilePosition end)
    {
        if (start.TileId == end.TileId) return 0;
        if (!_adjacency.ContainsKey(start.TileId) || !_adjacency.ContainsKey(end.TileId)) return -1;

        var visited = new HashSet<TileId>();
        var queue = new Queue<(TileId Id, int Dist)>();
        
        queue.Enqueue((start.TileId, 0));
        visited.Add(start.TileId);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();
            
            if (current == end.TileId) return dist;

            if (_adjacency.TryGetValue(current, out var neighbors))
            {
                foreach (var neighbor in neighbors)
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue((neighbor, dist + 1));
                    }
                }
            }
        }

        return -1; // Not connected
    }

    public bool IsConnected(TilePosition a, TilePosition b)
    {
        return GetDistance(a, b) != -1;
    }

    
}
