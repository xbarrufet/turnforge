using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

/// <summary>
/// Operation to move an entity to a new position.
/// </summary>
public sealed class MoveOperation : IGameStateOperation
{
    public EntityId Target { get; }
    public IBoardPosition NewPosition { get; }
    
    public MoveOperation(EntityId entityId, IBoardPosition newPosition)
    {
        Target = entityId;
        NewPosition = newPosition;
    }
    
    public void Apply(IGameStateMutator mutator)
    {
        mutator.SetPosition(Target, NewPosition);
    }
}
