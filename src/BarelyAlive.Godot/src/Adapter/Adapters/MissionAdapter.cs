using BarelyAlive.Rules.Game;
using Godot;

namespace BarelyAlive.Godot.Adapter.Adapters;

public class MissionAdapter
{
    private readonly BarelyAliveGame _game;

    public MissionAdapter(BarelyAliveGame game)
    {
        _game = game;
    }

    public BarelyAlive.Rules.Apis.Messaging.GameResponse LoadMissionFromJson(string missionJson)
    {
        return _game.BarelyAliveApis.InitializeGame(missionJson);
    }
    
    public async System.Threading.Tasks.Task<BarelyAlive.Rules.Apis.Messaging.GameResponse> LoadMissionInEngineAsync(BarelyAlive.Godot.Resources.Missions.MissionResource mission)
    {
        GD.Print($"[GodotAdapter] Loading mission: {mission.MissionName}");
        // 2. Call Engine Async
        GD.Print("[GodotAdapter] Initializing Engine (Async)...");

        return await System.Threading.Tasks.Task.Run(() =>
        {
                // Use the JSON directly from the mission resource
                return _game.BarelyAliveApis.InitializeGame(mission.JsonMissionData);
        });
    }
}
