namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;



public sealed class SpatialDto
{
    public string Type { get; init; } = default!;
    public List<PositionDto> Nodes { get; init; } = new();
    public List<ConnectionDto> Connections { get; init; } = new();
}

