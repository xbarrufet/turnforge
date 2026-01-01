using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

/// <summary>
/// Operation to spawn a new entity into the game state.
/// Records the entity and its initial position.
/// </summary>
public sealed class SpawnEntityOperation : IGameStateOperation
{
    public EntityId Target { get; }
    public Entities.GameEntity NewEntity { get; }
    public IBoardPosition? Position { get; }
    
    public SpawnEntityOperation(Entities.GameEntity entity) 
        : this(entity.Id, entity, null) { }
    
    public SpawnEntityOperation(EntityId id, GameEntity entity, IBoardPosition? position)
    {
        Target = id;
        NewEntity = entity;
        Position = position;
    }

    public void Apply(IGameStateMutator mutator)
    {
        mutator.AddEntity(NewEntity);
        
        if (Position != null)
        {
            mutator.SetPosition(Target, Position);
        }
    }
}