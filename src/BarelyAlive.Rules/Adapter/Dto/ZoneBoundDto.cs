
namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class ZoneBoundDto
{
    public string Type { get; init; } = default!;
    public List<PositionDto> Tiles { get; init; } = new();
}

