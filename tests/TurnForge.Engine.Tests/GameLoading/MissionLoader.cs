using System.Text.Json;
using TurnForge.Engine.Tests.GameLoading.Dto;
using System.IO;
    
namespace TurnForge.Engine.Tests.GameLoading;

public class MissionLoader
{
    
    public static MissionDto LoadFromFile(string fileName)
    {
        // If caller provided a path (contains directory separator or is rooted), use it as-is.
        string filePath;

        if (Path.IsPathRooted(fileName) || fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar))
        {
            filePath = fileName;
        }
        else
        {
            // Default location used previously
            filePath = Path.Combine("GameLoading", "Missions", fileName);

            // If the default location doesn't exist, try fallback to Assets/<fileName> so existing test assets work
            if (!File.Exists(filePath))
            {
                var alt = Path.Combine("Assets", fileName);
                if (File.Exists(alt)) filePath = alt;
            }
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Mission file not found: {filePath}");
        }

        var json = File.ReadAllText(filePath);
        var missionDto = JsonSerializer.Deserialize<MissionDto>(json);
        if (missionDto == null)
        {
            throw new InvalidOperationException($"Failed to deserialize mission from file: {filePath}");
        }

        return missionDto;
    }
}