using System.Text.Json.Serialization;

namespace urnForge.GodotAdapter.Dto.MissionDefinition
{

    public sealed class ConnectionDto
    {

        [JsonPropertyName("areaFromId")] public string AreaFromId { get; set; } = string.Empty;

        [JsonPropertyName("areaToId")] public string AreaToId { get; set; } = string.Empty;

        [JsonPropertyName("direction")] public string Direction { get; set; } = string.Empty;

        [JsonPropertyName("isOpen")] public bool IsOpen { get; set; }

        [JsonPropertyName("blockSight")] public bool BlockSight { get; set; }
    }
}