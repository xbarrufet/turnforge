using System.Text.Json.Serialization;

namespace TurnForge.Engine.Tests.GameLoading.Dto
{
    public sealed class ActorDto
    {
        [JsonPropertyName("actorKind")] public string ActorKind { get; set; } = string.Empty;
        [JsonPropertyName("position")] public string Position { get; set; } = string.Empty;
        [JsonPropertyName("actorId")] public string? ActorId { get; set; } = null;
        [JsonPropertyName("customType")] public string? CustomType { get; set; } = null;
        [JsonPropertyName("traits")] public List<TraitDto> Traits { get; set; } = [];
    }

    public sealed class TraitDto
    {
        [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
        [JsonPropertyName("Attributes")] public List<AttributeDto> Attributes { get; set; } = [];
    }

    public sealed class AttributeDto
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
    }
}
