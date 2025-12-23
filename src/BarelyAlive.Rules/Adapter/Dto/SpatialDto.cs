namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;

public sealed class SpatialDto
{
    public string Type { get; init; } = default!;
    public List<NodeDto> Nodes { get; init; } = new();
    public List<ConnectionDto> Connections { get; init; } = new();
    public List<ZoneDto> Zones { get; init; } = new();
}

