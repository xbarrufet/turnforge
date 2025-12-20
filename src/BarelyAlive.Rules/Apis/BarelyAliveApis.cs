
// expose end points to call teh run engine API¡s
using BarelyAlive.Rules.Adapter.Loaders;
using BarelyAlive.Rules.Apis.Interfaces;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Core.Interfaces;

namespace BarelyAlive.Rules.Apis;

public class BarelyAliveApis(IGameEngine gameEngine) : IBarelyAliveApis
{
    public BarelyAliveApiResult InitializeGame(string missionJson)
    {
        var (spatial, zones, props, agents) = MissionLoader.ParseMissionString(missionJson);
        var command = new InitGameCommand(spatial, zones, props, agents);
        var result = gameEngine.Send(command);
        return new BarelyAliveApiResult
        {
            Success = result.Success,
            Error = result.Error,
            Tags = result.Tags
        };
    }



}
