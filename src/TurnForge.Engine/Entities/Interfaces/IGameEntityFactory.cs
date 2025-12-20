using TurnForge.Engine.Entities.Descriptors.Interfaces;

namespace TurnForge.Engine.Entities.Factories.Interfaces;

public interface IGameEntityFactory<T> where T : GameEntity
{
    T Build(IGameEntityDescriptor<T> descriptor);
}