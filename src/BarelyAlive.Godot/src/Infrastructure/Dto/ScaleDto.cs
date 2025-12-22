using System.Text.Json.Serialization;

namespace BarelyAlive.Godot.Infrastructure.Dto
{
    public sealed class ScaleDto
    {
        [JsonPropertyName("width")] public int Width { get; set; }
        [JsonPropertyName("height")] public int Height { get; set; }
    }
}
