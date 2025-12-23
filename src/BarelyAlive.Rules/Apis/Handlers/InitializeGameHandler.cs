using BarelyAlive.Rules.Adapter.Loaders;
using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors;
using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Board.Descriptors;

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

    public (GameResponse Response, IReadOnlyList<SpawnRequest> Agents) Handle(string missionJson)
    {
        var (spatial, zones, props, agents) = MissionLoader.ParseMissionString(missionJson);
        
        // 1. Initialize Board (using command!)
        var boardDescriptor = new BoardDescriptor(spatial, zones);
        var initBoardCommand = new InitializeBoardCommand(boardDescriptor);
        _gameEngine.ExecuteCommand(initBoardCommand);  // Board setup (no response needed)

        // 2. Spawn Props using SpawnPropsCommand
        var spawnPropsCommand = new SpawnPropsCommand(props);
        var result = _gameEngine.ExecuteCommand(spawnPropsCommand);

        // Return projection from props spawn (board init is just setup)
        var payload = _projector.CreatePayload(result);

        var response = new GameResponse
        {
            TransactionId = result.Id,
            Success = result.Result.Success,
            Error = result.Result.Error,
            Payload = payload
        };

        // Return agent requests for later use in StartGame
        return (response, agents);
    }
}
