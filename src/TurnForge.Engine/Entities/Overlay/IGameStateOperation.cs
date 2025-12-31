using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

public interface IGameStateOperation
{

    EntityId Target { get; }
    void Apply(IGameStateMutator mutator);
}
