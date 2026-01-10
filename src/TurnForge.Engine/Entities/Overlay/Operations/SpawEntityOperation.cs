using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

/// <summary>
/// Operation to spawn a new entity into the game state.
/// Records the entity and its initial position.
/// </summary>
public record struct SpawnEntityOperation(GameEntity Entity, IBoardPosition? Position=null) : IGameStateOperation
{
    public EntityId EntityId => Entity.Id;
}