using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Interfaces;

namespace TurnForge.Engine.Decisions.Entity.Interfaces;

using TurnForge.Engine.Entities.Descriptors.Interfaces;

public interface IBuildDecision<T> : IDecision where T : GameEntity
{
    IGameEntityDescriptor<T> Descriptor { get; }
}
