using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class GameObject
{
    public ActorId Id { get; }
    private readonly Dictionary<Type, IActorComponent> _components = new();
    
    public void AddComponent<T>(T component) where T : IActorComponent
    {
        var type = typeof(T);
        if (_components.ContainsKey(type))
            throw new InvalidOperationException($"L'actor {Id} ja té un component de tipus {type.Name}");
        _components[type] = component;
    }

    public T? GetComponent<T>() where T : class, IActorComponent
    {
        return _components.TryGetValue(typeof(T), out var component)
            ? component as T
            : null;
    }

}