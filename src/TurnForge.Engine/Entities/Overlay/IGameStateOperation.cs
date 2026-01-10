using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

public interface IGameStateOperation
{
    EntityId EntityId { get; }
}