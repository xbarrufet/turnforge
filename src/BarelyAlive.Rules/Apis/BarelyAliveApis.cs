using BarelyAlive.Rules.Apis.Handlers;
using BarelyAlive.Rules.Apis.Interfaces;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors;
using TurnForge.Engine.APIs.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Descriptors;

namespace BarelyAlive.Rules.Apis;

public class BarelyAliveApis : IBarelyAliveApis
{
    private readonly InitializeGameHandler _initializeGameHandler;
    private readonly StartGameHandler _startGameHandler;
    private readonly GetSurvivorsHandler _getSurvivorsHandler;
    private readonly IGameEngine _gameEngine;

    private List<AgentDescriptor> _pendingAgents = new();

    public BarelyAliveApis(IGameEngine gameEngine, IGameCatalogApi catalog)
    {
        _gameEngine = gameEngine;
        var projector = new DomainProjector();
        _initializeGameHandler = new InitializeGameHandler(gameEngine, projector);
        _startGameHandler = new StartGameHandler(gameEngine, projector);
        _getSurvivorsHandler = new GetSurvivorsHandler(catalog);
    }

    public GameResponse InitializeGame(string missionJson)
    {
        var result = _initializeGameHandler.Handle(missionJson);
        _pendingAgents = result.Agents;
        return result.Response;
    }

    public GameResponse StartGame(string[] survivorIds)
    {
        var survivorDescriptors = survivorIds.Select(CreateSurvivorDescriptor).ToList();
        var allAgents = _pendingAgents.Concat(survivorDescriptors).ToList();
        
        var result = _startGameHandler.Handle(allAgents);
        _pendingAgents.Clear();
        return result;
    }

    private AgentDescriptor CreateSurvivorDescriptor(string typeId)
    {
        return new AgentDescriptor(
            typeId, // AgentName
            "Survivor",
            null // Position
        );
    }

    public List<SurvivorDefinition> GetAvailableSurvivors()
    {
        return _getSurvivorsHandler.Handle(new GetRegisteredSurvivorsQuery());
    }

    public void Ack()
    {
        _gameEngine.ExecuteCommand(new TurnForge.Engine.Commands.ACK.CommandAck());
    }
}
