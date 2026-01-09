using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

public record struct NexTurnResetApOperation(PlayerId PlayerId) : IGameStateOperation
{
    public EntityId EntityId => EntityId.Empty;
    
}