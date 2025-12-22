using System.Collections.Generic;
using BarelyAlive.Godot.Infrastructure.Dto;
using BarelyAlive.Godot.Resources.Missions;
using Godot;

namespace BarelyAlive.Godot.controllers;

public partial class MissionSetUpController:Node
{

    public void SetUpMission(string resourcePath)
    {
        try
        {
            GD.Print($"[MissionSetUp] SetUpMission called for {resourcePath}");
            var mission = GD.Load<MissionResource>(resourcePath);
            
            if (mission == null)
            {
                GD.PushError("[MissionSetUp] Mission resource not found: " + resourcePath);
                return;
            }
            
            GD.Print("[MissionSetUp] Loading mission resource: " + mission.MissionName);

            // 1. Prepare UI Context
            GD.Print("[MissionSetUp] deserializing mission DTO...");
            var missionDto = MapMissionDto(mission.JsonMissionData);
            var areaRectMap = GetAreaRectMap(missionDto);
            
            GD.Print($"[MissionSetUp] Setting MapContext (Size: {missionDto.MapSize.Width}x{missionDto.MapSize.Height}, Areas: {areaRectMap.Count})...");
            GameSession.Instance.MapContext.Set(
                mission.JsonMissionData,
                missionDto.MissionName,
                new Vector2(missionDto.MapSize.Width, missionDto.MapSize.Height),
                mission.Map,
                areaRectMap
            );
            GD.Print("[MissionSetUp] MapContext Set called.");
            
            // 2. Initialize Engine
            GD.Print("[MissionSetUp] Calling Adapter.LoadMission...");
            BarelyAlive.Godot.Adapter.TurnForgeAdapter.Instance.LoadMission(mission);
            GD.Print("[MissionSetUp] Adapter.LoadMission called.");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"[MissionSetUp] CRITICAL ERROR IN SETUP: {ex.Message}\n{ex.StackTrace}");
        }
    }
    
    private Dictionary<string, Rect2> GetAreaRectMap(MissionDto mission)
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
