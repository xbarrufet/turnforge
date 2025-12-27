using TurnForge.Engine.Definitions.Descriptors.Interfaces;

namespace TurnForge.Engine.Definitions.Factories.Interfaces;

public interface IGameEntityFactory<T> where T : GameEntity
{
    T Build(IGameEntityDescriptor<T> descriptor);
}