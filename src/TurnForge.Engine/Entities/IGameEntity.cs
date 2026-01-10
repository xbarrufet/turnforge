using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public interface IGameEntity
{
    EntityId Id { get; }
    string DefinitionId { get; }
    string Name { get; }
    Category Category { get; }
    public bool HasComponent<T>() where T : class, IGameEntityComponent;
    public T GetRequiredComponent<T>() where T : class, IGameEntityComponent;

    public T? GetTrait<T>() where T : class, ITrait;
    public bool HasTrait<T>() where T : class, ITrait;
    public void AddTrait<T>(T trait) where T : class, ITrait;
    public bool RemoveTrait<T>() where T : class, ITrait;
    public IEnumerable<ITrait> GetAllTraits();
    public T GetRequiredTrait<T>() where T : class, ITrait;

}
