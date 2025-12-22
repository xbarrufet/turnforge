using System.Collections.Generic;
using BarelyAlive.Godot.TurnForge.GodotAdapter.Adapters;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Game;
using Godot;

namespace BarelyAlive.Godot.TurnForge.GodotAdapter;

public partial class GodotAdapter : Node
{
    public static GodotAdapter Instance { get; private set; }
    private BarelyAliveGame _engineRuntime;
    
    public bool IsInitialized => _engineRuntime != null;

    public MissionAdapter Mission { get; private set; }

    [Signal]
    public delegate void GameInitializedEventHandler(bool success);

    public override void _EnterTree()
    {
        Instance = this;
    }

    public void Bootstrap()
    {
        GD.Print("Bootstrap started");
        _engineRuntime = BarelyAliveGame.CreateNewGame();
        Mission = new MissionAdapter(_engineRuntime);
        GD.Print("Bootstrap completed");
    }

    public List<SurvivorDefinition> GetAvailableSurvivors()
    {
        if (_engineRuntime == null)
        {
            GD.PushWarning("[GodotAdapter] Engine not initialized. Cannot get survivors.");
            return new List<SurvivorDefinition>();
        }
        return _engineRuntime.BarelyAliveApis.GetAvailableSurvivors();
    }

    public async void LoadMission(BarelyAlive.Godot.Resources.Missions.MissionResource mission)
    {
        GD.Print($"[GodotAdapter] Loading mission: {mission.MissionName}");

        try
        {
            // 1. Populate UI Model (GameContext) - Keep on Main Thread
            var areas = ParseAreasFrom(mission.JsonMissionData);

            GameSession.Instance.SetMissionContext(
                mission.MissionName,
                mission.MapSize,
                mission.Map,
                areas
            );

            // 2. Call Engine Async
            GD.Print("[GodotAdapter] Initializing Engine (Async)...");

            var response = await System.Threading.Tasks.Task.Run(() =>
            {
                return Mission.LoadMissionFromJson(mission.JsonMissionData);
            });

            // 3. Back on Main Thread
            if (response.Success)
            {
                GD.Print($"[GodotAdapter] Engine Initialized! Created: {response.Payload?.Created?.Count ?? 0} entities.");
                EmitSignal(global::TurnForge.GodotAdapter.GodotAdapter.SignalName.GameInitialized, true);
            }
            else
            {
                GD.PrintErr($"[GodotAdapter] Engine Initialization failed: {response.Error}");
                EmitSignal(global::TurnForge.GodotAdapter.GodotAdapter.SignalName.GameInitialized, false);
            }
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[GodotAdapter] Failed to load mission: {e}");
            EmitSignal(global::TurnForge.GodotAdapter.GodotAdapter.SignalName.GameInitialized, false);
        }
    }

    private Dictionary<string, Rect2> ParseAreasFrom(string json)
    {
        var rects = new Dictionary<string, Rect2>();
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        
        // Try legacy "areas"
        if (doc.RootElement.TryGetProperty("areas", out var areasArray))
        {
            foreach (var element in areasArray.EnumerateArray())
            {
                string id = element.GetProperty("id").GetString() ?? System.Guid.NewGuid().ToString();
                float x = element.GetProperty("x").GetSingle();
                float y = element.GetProperty("y").GetSingle();
                float w = element.GetProperty("width").GetSingle();
                float h = element.GetProperty("height").GetSingle();
                rects[id] = new Rect2(x, y, w, h);
            }
        }
        // Try new "zones"
        else if (doc.RootElement.TryGetProperty("zones", out var zonesArray))
        {
            foreach (var element in zonesArray.EnumerateArray())
            {
                string id = element.GetProperty("id").GetString() ?? System.Guid.NewGuid().ToString();
                
                // Case sensitive check for "Area" or "area"
                System.Text.Json.JsonElement areaElem;
                if (element.TryGetProperty("Area", out areaElem) || element.TryGetProperty("area", out areaElem))
                {
                    float x = areaElem.GetProperty("x").GetSingle();
                    float y = areaElem.GetProperty("y").GetSingle();
                    float w = areaElem.GetProperty("width").GetSingle();
                    float h = areaElem.GetProperty("height").GetSingle();
                    rects[id] = new Rect2(x, y, w, h);
                }
            }
        }
        return rects;
    }
}
