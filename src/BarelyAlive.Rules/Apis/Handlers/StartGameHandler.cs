using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Descriptors;

namespace BarelyAlive.Rules.Apis.Handlers;

public class StartGameHandler
{
    private readonly IGameEngine _gameEngine;
    private readonly DomainProjector _projector;

    public StartGameHandler(IGameEngine gameEngine, DomainProjector projector)
    {
        _gameEngine = gameEngine;
        _projector = projector;
    }

    public GameResponse Handle(List<AgentDescriptor> agents)
    {
        var command = new StartGameCommand(agents);
        var result = _gameEngine.ExecuteCommand(command);

        var payload = _projector.CreatePayload(result);

        return new GameResponse
        {
            TransactionId = result.Id,
            Success = result.Result.Success,
            Error = result.Result.Error,
            Payload = payload
        };
    }
}
