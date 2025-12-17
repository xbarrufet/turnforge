using System.Collections.Generic;
using BarelyAlive.Godot.Infrastructure.Dto;
using BarelyAlive.Godot.Resources.Missions;
using Godot;

namespace BarelyAlive.Godot.Infrastructure;

public partial class MissionLoader:Node
{
    
    
    public void LoadMissionFromResourcePath(string resourcePath)
    {
        var mission = GD.Load<MissionResource>(resourcePath);
        
        if (mission == null)
        {
            GD.PushError("Mission resource not found");
            return;
        }
        GD.Print("Loading mission: " + mission.MissionName);
        //extract mission Data
        var missionDto = MapMissionDto(mission.JsonMissionData);
        var areaRectMap = GetAreaRectMap(missionDto);   
        GameContext.Instance.SetMissionContext(
            missionDto.MissionName,
            ParseUtil(missionDto.MapSize),
            mission.Map,
            areaRectMap
        );

    }
    
    private Vector2 ParseUtil(string vectorString)
    {
        var parts = vectorString.Split('x');
        if (parts.Length != 2)
        {
            throw new System.FormatException("Invalid vector format");
        }
        return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
    }
    
    Dictionary<string, Rect2> GetAreaRectMap(MissionDto mission)
    {
        var areaRectMap = new Dictionary<string, Rect2>();
        foreach (var area in mission.Areas)
        {
            areaRectMap[area.Id] = new Rect2(
                new Vector2(area.X, area.Y),
                new Vector2(area.Width, area.Height)
            );
        }
        return areaRectMap;
    }

    MissionDto MapMissionDto(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<MissionDto>(json)
               ?? throw new System.InvalidOperationException("Failed to deserialize mission JSON");
    }
}