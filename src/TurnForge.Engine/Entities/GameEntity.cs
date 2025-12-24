using TurnForge.Engine.Behaviours.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

using TurnForge.Engine.Entities.Interfaces;

namespace TurnForge.Engine.Entities;

public abstract class GameEntity : IGameEntity, IComponentContainer
{
    public EntityId Id { get; }
    public string DefinitionId { get; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    protected GameEntity(EntityId id, string name, string category, string definitionId)
    {
        Id = id;
        Name = name;
        Category = category;
        DefinitionId = definitionId;
    }

    private readonly Dictionary<Type, IGameEntityComponent> _components = new()
    {
        { typeof(IBehaviourComponent), new BaseBehaviourComponent() }
    };

    public IReadOnlyCollection<IGameEntityComponent> Components => _components.Values;

    
    public void AddComponent<T>(T component) where T : IGameEntityComponent
    {
        var type = typeof(T);
        
        if (component is BaseBehaviourComponent behaviourComponent)
        {
            behaviourComponent.SetOwner(this);
        }
        else
        {
            if (_components.ContainsKey(type))
                throw new InvalidOperationException($"L'entitat {Id} ja té un component de tipus {type.Name}");

        }
        _components[type] = component;
    }
    
    public bool HasComponent<T>() where T : class, IGameEntityComponent
    {
        return _components.ContainsKey(typeof(T));
    }
    
    public T? GetComponent<T>() where T : class, IGameEntityComponent
    {
        return GetComponent(typeof(T)) as T;
    }

    public IBehaviourComponent GetBehaviourComponent()
    {
        return GetRequiredComponent<IBehaviourComponent>();
    }

    public T GetRequiredComponent<T>() where T : class, IGameEntityComponent
    {
        var component = GetComponent<T>();
        if (component == null)
        {
            throw new InvalidOperationException($"Entity {Id} missing required component {typeof(T).Name}");
        }
        return component;
    }

    public bool TryGetComponent<T>(out T? component) where T : class, IGameEntityComponent
    {
        if (_components.TryGetValue(typeof(T), out var c) && c is T typedComponent)
        {
            component = typedComponent;
            return true;
        }
        component = null;
        return false;
    }

  public IGameEntityComponent? GetComponent(Type componentType)
{
    // First, try direct lookup
    if (_components.TryGetValue(componentType, out var component))
    {
        return component;
    }
    
    // If not found, search by interface/base class
    foreach (var kvp in _components)
    {
        // Check if the registered component type implements/inherits the requested type
        if (componentType.IsAssignableFrom(kvp.Key))  // ← AIXÒ ÉS CORRECTE
        {
            Console.WriteLine($"[GetComponent] Found {kvp.Key.Name} for requested {componentType.Name}");
            return kvp.Value;
        }
    }
    
    Console.WriteLine($"[GetComponent] NOT FOUND for {componentType.Name}. Registered: {string.Join(", ", _components.Keys.Select(k => k.Name))}");
    return null;
}

    public virtual bool HasRequiredComponents()
    {
        return _components.ContainsKey(typeof(IBehaviourComponent));
    }

    public bool HasBehavior<T>() where T : IBaseBehaviour
    {
        return GetBehaviourComponent().HasBehaviour<T>();
    }

    public string GetComponents()
    {
        return string.Join(", ", _components.Values.Select(c => c.GetType().Name));
    }

    public IEnumerable<IGameEntityComponent> GetAllComponents()
    {
        return _components.Values;
    }
}
