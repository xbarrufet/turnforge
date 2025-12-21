using BarelyAlive.Rules.Apis.Handlers;
using BarelyAlive.Rules.Apis.Interfaces;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors;
using TurnForge.Engine.Core.Interfaces;

namespace BarelyAlive.Rules.Apis;

public class BarelyAliveApis : IBarelyAliveApis
{
    private readonly InitializeGameHandler _initializeGameHandler;
    private readonly StartGameHandler _startGameHandler;

    private List<TurnForge.Engine.Entities.Descriptors.AgentDescriptor> _pendingAgents = new();

    public BarelyAliveApis(IGameEngine gameEngine)
    {
        var projector = new DomainProjector();
        _initializeGameHandler = new InitializeGameHandler(gameEngine, projector);
        _startGameHandler = new StartGameHandler(gameEngine, projector);
    }

    public GameResponse InitializeGame(string missionJson)
    {
        var result = _initializeGameHandler.Handle(missionJson);
        _pendingAgents = result.Agents;
        return result.Response;
    }

    public GameResponse StartGame()
    {
        var result = _startGameHandler.Handle(_pendingAgents);
        _pendingAgents.Clear();
        return result;
    }
}
