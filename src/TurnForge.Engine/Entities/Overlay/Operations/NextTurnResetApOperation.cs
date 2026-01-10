using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

public record struct NextTurnResetApOperation() : IGameStateOperation
{
    public EntityId EntityId => EntityId.Empty;
    
}