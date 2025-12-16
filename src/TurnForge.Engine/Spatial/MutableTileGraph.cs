using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace TurnForge.Engine.Spatial;

public sealed class MutableTileGraph : IMutableTileGraph
{
    private readonly Dictionary<Position, HashSet<Position>> _adjacency;

    public MutableTileGraph(IEnumerable<(Position A, Position B)> connections)
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

    public void EnableEdge(Position from, Position to)
    {
        if (!_adjacency.TryGetValue(from, out var set))
        {
            set = new HashSet<Position>();
            _adjacency[from] = set;
        }

        set.Add(to);
    }

    public void DisableEdge(Position from, Position to)
    {
        if (_adjacency.TryGetValue(from, out var set))
        {
            set.Remove(to);
        }
    }

    // ───────────────
    // QUERIES
    // ───────────────

    public bool Exists(Position tile)
        => _adjacency.ContainsKey(tile);

    public bool AreAdjacent(Position from, Position to)
        => _adjacency.TryGetValue(from, out var set)
           && set.Contains(to);

    public IEnumerable<Position> GetNeighbors(Position tile)
        => _adjacency.TryGetValue(tile, out var set)
            ? set
            : Enumerable.Empty<Position>();

    public int ShortestPathLength(Position from, Position to)
    {
        if (from.Equals(to))
            return 0;

        var visited = new HashSet<Position> { from };
        var queue = new Queue<(Position tile, int dist)>();
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
