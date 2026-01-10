using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

/// <summary>
/// Operation to move an entity to a new position.
/// </summary>
public record struct MoveOperation(EntityId EntityId, IBoardPosition NewPosition) : IGameStateOperation
{
}
