using System.Text.Json;
using TurnForge.Rules.BarelyAlive.Dto;

namespace TurnForge.Rules.BarelyAlive.Loading;

public class MissionLoader
{
    public static MissionDto LoadFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var dto = JsonSerializer.Deserialize<MissionDto>(json, options);
        return dto ?? throw new InvalidOperationException("Failed to deserialize mission.");
    }
}
