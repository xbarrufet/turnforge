using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Spatial;

public sealed class MutableTileGraph : IMutableTileGraph
{
    private readonly Dictionary<TileId, HashSet<TileId>> _adjacency;
    private readonly HashSet<TileId> _nodes;

    public MutableTileGraph(HashSet<TileId> nodes)
    {
        _adjacency = new();
        _nodes = nodes;
    }

    public void AddAjacency(TileId from, TileId to)
    {
        EnableEdge(from, to);
    }

    public MutableTileGraph(IEnumerable<(TileId A, TileId B)> connections)
    {
        _adjacency = new();
        foreach (var (a, b) in connections)
        {
            EnableEdge(a, b);
            EnableEdge(b, a);
        }
    }




    // ───────────────
    // MUTATIONS
    // ───────────────

    public void EnableEdge(TileId from, TileId to)
    {
        if (!_adjacency.TryGetValue(from, out var set))
        {
            set = new HashSet<TileId>();
            _adjacency[from] = set;
        }

        set.Add(to);
    }

    public void DisableEdge(TileId from, TileId to)
    {
        if (_adjacency.TryGetValue(from, out var set))
        {
            set.Remove(to);
        }
    }

    // ───────────────
    // QUERIES
    // ───────────────

    public bool Exists(TileId tile)
        => _adjacency.ContainsKey(tile);

    public bool AreAdjacent(TileId from, TileId to)
        => _adjacency.TryGetValue(from, out var set)
           && set.Contains(to);

    public IEnumerable<TileId> GetNeighbors(TileId tile)
        => _adjacency.TryGetValue(tile, out var set)
            ? set
            : Enumerable.Empty<TileId>();

    public int ShortestPathLength(TileId from, TileId to)
    {
        if (from.Equals(to))
            return 0;

        var visited = new HashSet<TileId> { from };
        var queue = new Queue<(TileId tile, int dist)>();
        queue.Enqueue((from, 0));

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            foreach (var n in GetNeighbors(current))
            {
                if (visited.Contains(n))
                    continue;

                if (n.Equals(to))
                    return dist + 1;

                visited.Add(n);
                queue.Enqueue((n, dist + 1));
            }
        }

        return int.MaxValue;
    }
}
