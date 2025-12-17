using System.Text.Json.Serialization;

namespace urnForge.GodotAdapter.Dto.MissionDefinition
{

    public sealed class AreaDto
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

        [JsonPropertyName("x")] public int X { get; set; }

        [JsonPropertyName("y")] public int Y { get; set; }

        [JsonPropertyName("width")] public int Width { get; set; }

        [JsonPropertyName("height")] public int Height { get; set; }

        [JsonPropertyName("isIndoor")] public bool IsIndoor { get; set; }

        [JsonPropertyName("isDark")] public bool IsDark { get; set; }
    }
}