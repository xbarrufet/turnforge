using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public abstract class GameEntity(EntityId id)
{
    public EntityId Id { get; } = id;
    private readonly Dictionary<Type, IGameEntityComponent> _components = new();

    public void AddComponent<T>(T component) where T : IGameEntityComponent
    {
        var type = typeof(T);
        if (_components.ContainsKey(type))
            throw new InvalidOperationException($"L'entitat {Id} ja té un component de tipus {type.Name}");

        if (component is Components.BehaviourComponent behaviourComponent)
        {
            behaviourComponent.SetOwner(this);
        }

        _components[type] = component;
    }

    public T? GetComponent<T>() where T : class, IGameEntityComponent
    {
        return _components.TryGetValue(typeof(T), out var component)
            ? component as T
            : null;
    }
}
