using System.Text.Json.Serialization;

namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;

public sealed class PropDto
{
    public string TypeId { get; init; } = default!;
    
    [JsonPropertyName("Behaviours")]
    public List<TraitDto> Traits { get; init; } = new();
    
    public int MaxHealth { get; init; }
    public int MaxBaseMovement { get; init; }
    public int MaxActionPoints { get; init; }
    public object? Position { get; init; }
}

