using BarelyAlive.Rules.Adapter.Dto;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;

namespace BarelyAlive.Rules.Adapter.Mappers;

public static class SpatialModelMapper
{
    public static ISpatialModel BuildSpatialModel(SpatialDto dto)
    {
        if (dto.Type != "DiscreteGraph")
            throw new NotSupportedException($"Spatial type '{dto.Type}'");

        var nodes = dto.Nodes
            .Select(n => n.ToPosition())
            .ToHashSet();

        var edges = dto.Connections
            .Select(c => (
                From: c.From.ToPosition(),
                To: c.To.ToPosition()
            ))
            .ToList();

        var grap= new MutableTileGraph(nodes);
        foreach (var edge in edges)
        {
            grap.EnableEdge(edge.From, edge.To);
        }

        return new ConnectedGraphSpatialModel(grap);
    }
}

