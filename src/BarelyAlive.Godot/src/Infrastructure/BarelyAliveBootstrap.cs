using BarelyAlive.Godot.Resources.Missions;
using TurnForge.GodotAdapter;
using Godot;
using TurnForge.Adapters.Godot;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories.InMemory;
using TurnForge.GodotAdapter;
using TurnForge.Rules.BarelyAlive.Actors;

namespace BarelyAlive.Godot.Infrastructure;

public partial class BarelyAliveBootstrap:Node
{
    
    [Export]
    public string AutoloadMissionPath { get; set; } = 
        "res://src/resources/Missions/mission01.tres";
    
    public override void _Ready()
    {
       // var gameContext = new GameContext();
       //AddChild(gameContext);
        var gameEngineContext= BuildGameEngineContext();
        GodotAdapter.Instance.Bootstrap(gameEngineContext);
        if (!string.IsNullOrEmpty(AutoloadMissionPath))
        {
            var missionLoader = GetNode<MissionLoader>("MissionLoader");
            missionLoader.LoadMissionFromResourcePath(AutoloadMissionPath);
        }
    }

    private GameEngineContext BuildGameEngineContext()
    {
        return new GameEngineContext(new BarelyAliveActorFactory(), 
            new InMemoryGameRepository());
    }
    
}