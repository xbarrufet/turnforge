using BarelyAlive.Rules.Game;
using Godot;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Core;
using TurnForge.Engine.Infrastructure;

namespace TurnForge.GodotAdapter;

public partial class GodotAdapter:Node
{
    private BarelyAliveGame _engineRuntime;
    public static GodotAdapter Instance { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
    }
    
    public void Bootstrap(GameEngineContext context)
    {
        GD.Print("Bootstrap started");
        _engineRuntime = BarelyAliveGame.CreateNewGame();
        GD.Print("Bootstrap completed");
        
    }
    public void LoadDiscreteMission(DiscreteSpatialMissionDefinitionDto missionDefinitionDto )
    {
        // Map DTO -> Engine definitions
        var spatial = MissionDefinitionMapper.FromDto(missionDefinitionDto);
        var actors = MissionDefinitionMapper.FromDto(missionDefinitionDto.Actors);

        var command = new InitializeGameCommand(
            spatial: spatial,
            actors: actors
        );

        // Use engine API
        _engineRuntime.InitializeGame(command);
    }

    /*public void LoadMission(MissionDefinitionDTO mission)
    {
        _engine.CommandBus.Dispatch(
            new LoadMissionCommand(mission.Id, mission.Board)
        );
    }

    public void StartGame(PartyDefinitionDTO party)
    {
        _engine.CommandBus.Dispatch(
            new GameStartCommand(party.Units)
        );
    }*/
}
