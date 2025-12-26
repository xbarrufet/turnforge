using System.Text.Json.Serialization;

namespace BarelyAlive.Rules.Adapter.Dto;

using System.Collections.Generic;

public sealed class AgentDto
{
    public string AgentName { get; init; } = default!;
    public string Category { get; init; } = "Survivor";
    
    [JsonPropertyName("Behaviours")]
    public List<TraitDto> Traits { get; init; } = new();
    
    public int MaxHealth { get; init; }
    public int MaxBaseMovement { get; init; }
    public int MaxActionPoints { get; init; }
    public PositionDto? Position { get; init; }
}
