using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.ValueObjects;
// For MissionDefinition
// For IGameStateOperation

// For EntityId

namespace TurnForge.Engine.Entities.Overlay.Operations;

public record struct CreateBoardOperation(IGameBoard Board) : IGameStateOperation
{

    public EntityId EntityId => EntityId.Empty;
}
