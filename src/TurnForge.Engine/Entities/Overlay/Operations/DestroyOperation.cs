using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

public record struct DestroyOperation(EntityId EntityId) : IGameStateOperation
{
    public EntityId EntityId { get; } = EntityId;
}