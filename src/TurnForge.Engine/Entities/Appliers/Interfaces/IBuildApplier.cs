using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;

namespace TurnForge.Engine.Entities.Appliers.Interfaces;

public interface IBuildApplier<T> where T : GameEntity
{
    T Build(IGameEntityDescriptor<T> descriptor, IGameEntityFactory<T> factory);
}