
namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class MissionDto
{
    public string MissionName { get; init; } = default!;
    public ScaleDto Scale { get; init; } = default!;
    public SpatialDto Spatial { get; init; } = default!;
    public List<ZoneDto> Zones { get; init; } = new();
    public List<PropDto> Props { get; init; } = new();
}

