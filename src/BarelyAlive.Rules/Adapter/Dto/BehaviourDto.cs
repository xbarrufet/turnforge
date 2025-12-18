using System.Text.Json;

namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class BehaviourDto
{
    public string Type { get; init; } = default!;
    public JsonElement Data { get; init; }
}

