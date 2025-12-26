using System.Collections.Generic;
using System.Text.Json;

namespace BarelyAlive.Rules.Adapter.Dto;

public sealed class TraitDto
{
    public string Type { get; init; } = default!;

    [System.Text.Json.Serialization.JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; init; } = new();
}

