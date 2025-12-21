using BarelyAlive.Rules.Adapter.Loaders;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Core.Interfaces;

namespace BarelyAlive.Rules.Apis.Handlers;

public class InitializeGameHandler
{
    private readonly IGameEngine _gameEngine;
    private readonly DomainProjector _projector;

    public InitializeGameHandler(IGameEngine gameEngine, DomainProjector projector)
    {
        _gameEngine = gameEngine;
        _projector = projector;
    }

    public (GameResponse Response, List<TurnForge.Engine.Entities.Descriptors.AgentDescriptor> Agents) Handle(string missionJson)
    {
        var (spatial, zones, props, agents) = MissionLoader.ParseMissionString(missionJson);
        var command = new InitGameCommand(spatial, zones, props);
        var result = _gameEngine.ExecuteCommand(command);

        var payload = _projector.CreatePayload(result);

        var response = new GameResponse
        {
            TransactionId = result.Id,
            Success = result.Result.Success,
            Error = result.Result.Error,
            Payload = payload
        };
        return (response, agents.ToList());
    }
}
