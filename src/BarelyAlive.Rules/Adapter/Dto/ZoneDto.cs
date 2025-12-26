using System.Text.Json.Serialization;

namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;

public sealed class ZoneDto
{
    public string Id { get; init; } = default!;
    public ZoneBoundDto Bound { get; init; } = default!;
    
    [JsonPropertyName("Behaviours")]
    public List<TraitDto> Traits { get; init; } = new();
}

