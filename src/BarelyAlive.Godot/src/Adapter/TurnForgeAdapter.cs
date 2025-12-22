using System.Collections.Generic;
using BarelyAlive.Godot.Adapter.Adapters;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Game;
using Godot;

namespace BarelyAlive.Godot.Adapter;

public partial class TurnForgeAdapter : Node
{
    public static TurnForgeAdapter Instance { get; private set; }
    private BarelyAliveGame _engineRuntime;
    
    public bool IsInitialized => _engineRuntime != null;

    public MissionAdapter Mission { get; private set; }
    public QueryCatalogAdapter QueryCatalog { get; private set; }

    [Signal]
    public delegate void GameInitializedEventHandler(bool success);

    public override void _EnterTree()
    {
        Instance = this;
    }

    public void Bootstrap()
    {
        GD.Print("Bootstrap started");
        _engineRuntime = BarelyAliveGame.CreateNewGame(new Infrastructure.GodotLogger());
        Mission = new MissionAdapter(_engineRuntime);
        QueryCatalog = new QueryCatalogAdapter(_engineRuntime);
        GD.Print("Bootstrap completed");
    }

    public List<SurvivorDefinition> GetAvailableSurvivors()
    {
        if (_engineRuntime == null)
        {
            GD.PushWarning("[GodotAdapter] Engine not initialized. Cannot get survivors.");
            return new List<SurvivorDefinition>();
        }
        return QueryCatalog.GetSurvivorsFromCatalog();
    }
    public void StartGame(string[] survivorIds)
    {
        if (_engineRuntime == null)
        {
            GD.PushWarning("[GodotAdapter] Engine not initialized. Cannot start game.");
            return;
        }

        GD.Print($"[GodotAdapter] Starting game with {survivorIds.Length} survivors...");
        var response = _engineRuntime.BarelyAliveApis.StartGame(survivorIds);
        
        if (response.Success)
        {
             GD.Print($"[GodotAdapter] Game Started Successfully! Entities: {response.Payload?.Created?.Count ?? 0}");
             // Potentially emit another signal here like GameStarted
        }
        else
        {
            
            GD.PrintErr($"[GodotAdapter] Failed to start game: {response.Error}");
        }
    }
  

    public async void LoadMission(BarelyAlive.Godot.Resources.Missions.MissionResource mission)
    {
        try
        {
            var response = await Mission.LoadMissionInEngineAsync(mission);
            
            if (response.Success)
            {
                 GD.Print($"[GodotAdapter] Engine Initialized! Created: {response.Payload?.Created?.Count ?? 0} entities.");
                 EmitSignal(SignalName.GameInitialized, true);
                 _engineRuntime.BarelyAliveApis.Ack();
            }
            else
            {
                 GD.PrintErr($"[GodotAdapter] Engine Initialization failed: {response.Error}");
                 EmitSignal(SignalName.GameInitialized, false);
            }
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[GodotAdapter] Failed to load mission: {e}");
            EmitSignal(SignalName.GameInitialized, false);
        }
    }
}
