using TurnForge.Engine.ValueObjects;

namespace TurnForge.Rules.BarelyAlive.Dto;

public class ZoneBoundDto
{
    public string Type { get; set; } = string.Empty;
    public int? X { get; set; }
    public int? Y { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public List<Position>? Tiles { get; set; }
}
