using System.Text.Json.Serialization;

namespace TurnForge.Engine.Tests.GameLoading.Dto
{

    public sealed class MissionDto
    {
        [JsonPropertyName("missionName")] public string MissionName { get; set; } = string.Empty;

        [JsonPropertyName("scale")] public string MapSize { get; set; } = string.Empty;

        [JsonPropertyName("areas")] public List<AreaDto> Areas { get; set; } = [];

        [JsonPropertyName("areaConnections")] public List<ConnectionDto> Connections { get; set; } = [];
    }
}