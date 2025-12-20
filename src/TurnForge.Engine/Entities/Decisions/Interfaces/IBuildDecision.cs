using TurnForge.Engine.Entities.Appliers.Interfaces;

namespace TurnForge.Engine.Entities.Decisions.Interfaces;

using TurnForge.Engine.Entities.Descriptors.Interfaces;

public interface IBuildDecision<T> : IDecision where T : GameEntity
{
    IGameEntityDescriptor<T> Descriptor { get; }
}
