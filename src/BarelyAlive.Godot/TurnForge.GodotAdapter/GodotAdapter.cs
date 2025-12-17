using Godot;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Core;
using TurnForge.Engine.Infrastructure;
using TurnForge.GodotAdapter.Dto.MissionDefinition;

namespace TurnForge.GodotAdapter;

public partial class GodotAdapter:Node
{
    private GameEngine _engine;
    public static GodotAdapter Instance { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
    }
    
    public void Bootstrap(GameEngineContext context)
    {
        GD.Print("Bootstrap started");
        _engine = GameEngineFactory.Build(context);
        GD.Print("Bootstrap completed");
        
    }
    public void LoadDiscreteMission(DiscreteSpatialMissionDefinitionDto missionDefinitionDto )
    {
        // Map DTO -> Engine definitions
        var spatial = MissionDefinitionMapper.FromDto(missionDefinitionDto);
        var actors = MissionDefinitionMapper.FromDto(missionDefinitionDto.Actors);

        var command = new LoadGameCommand(
            spatial: spatial,
            actors: actors
        );

        // Use engine API
        _engine.LoadGame(command);
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
