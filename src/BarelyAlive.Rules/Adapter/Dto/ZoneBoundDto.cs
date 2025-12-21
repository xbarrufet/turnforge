namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;

public sealed class ZoneBoundDto
{
    public string Type { get; init; } = default!;
    public List<PositionDto> Tiles { get; init; } = new();
}

