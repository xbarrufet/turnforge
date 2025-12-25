using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors;
using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Apis.Handlers;

/// <summary>
/// Handles movement commands for any character (Survivor, Zombie, etc.).
/// Uses TurnForge's MoveCommand internally.
/// </summary>
public class MoveCharacterHandler
{
    private readonly IGameEngine _gameEngine;
    private readonly DomainProjector _projector;

    public MoveCharacterHandler(IGameEngine gameEngine, DomainProjector projector)
    {
        _gameEngine = gameEngine;
        _projector = projector;
    }

    public GameResponse Handle(string characterId, TileReference targetTile)
    {
        var tileId = new TileId(Guid.Parse(targetTile.Id));
        var position = new Position(tileId);
        var command = new MoveCommand(characterId, hasCost: true, targetPosition: position);
        
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
