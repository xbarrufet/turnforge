using System.Text.Json.Serialization;

namespace TurnForge.Rules.BarelyAlive.Dto;

public class AreaDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("isIndoor")]
    public bool IsIndoor { get; set; }

    [JsonPropertyName("isDark")]
    public bool IsDark { get; set; }
}

public class ActorPlacementDto
{
    [JsonPropertyName("actorId")]
    public string? ActorId { get; set; }

    [JsonPropertyName("actorKind")]
    public string ActorKind { get; set; } = string.Empty;

    [JsonPropertyName("position")]
    public string PositionAreaId { get; set; } = string.Empty;

    [JsonPropertyName("customType")]
    public string CustomType { get; set; } = string.Empty;

    public List<TraitDto> Behaviours { get; set; } = new();
}

public class MissionDto
{
    [JsonPropertyName("missionName")]
    public string MissionName { get; set; } = string.Empty;

    [JsonPropertyName("scale")]
    public string Scale { get; set; } = string.Empty;

    [JsonPropertyName("areas")]
    public List<AreaDto> Areas { get; set; } = new();

    [JsonPropertyName("Actors")]
    public List<ActorPlacementDto> Actors { get; set; } = new();
}
