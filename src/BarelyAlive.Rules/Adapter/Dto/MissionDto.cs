namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;



public sealed class MissionDto
{
    public string MissionName { get; init; } = default!;
    public ScaleDto Scale { get; init; } = default!;
    public SpatialDto Spatial { get; init; } = default!;
    public List<PropDto> Props { get; init; } = new();
    public List<AgentDto> Agents { get; init; } = new();
}
