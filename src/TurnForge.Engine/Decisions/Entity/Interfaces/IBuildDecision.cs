using TurnForge.Engine.Definitions;
using TurnForge.Engine.Appliers.Entity.Interfaces;

namespace TurnForge.Engine.Decisions.Entity.Interfaces;

using TurnForge.Engine.Definitions.Descriptors.Interfaces;

public interface IBuildDecision<T> : IDecision where T : GameEntity
{
    IGameEntityDescriptor<T> Descriptor { get; }
}
