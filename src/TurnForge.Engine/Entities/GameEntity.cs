using TurnForge.Engine.Traits.Interfaces;
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
    
    /// <summary>
    /// Team/Faction this entity belongs to (e.g., "Survivors", "Zombies", "Orcs").
    /// Used to determine allies vs enemies in combat.
    /// </summary>
    public string Team { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the controller (player or AI) that controls this entity.
    /// Null or empty means no specific controller (e.g., neutral).
    /// </summary>
    public string? ControllerId { get; set; }

    protected GameEntity(EntityId id, string name, string category, string definitionId)
    {
        Id = id;
        Name = name;
        Category = category;
        DefinitionId = definitionId;
    }

    private readonly Dictionary<Type, IGameEntityComponent> _components = new()
    {
        { typeof(ITraitContainerComponent), new TraitContainerComponent() }
    };

    public IReadOnlyCollection<IGameEntityComponent> Components => _components.Values;

    
    public virtual void AddComponent<T>(T component) where T : IGameEntityComponent
    {
        var type = component.GetType();
        
        if (component is TraitContainerComponent traitComponent)
        {
            traitComponent.SetOwner(this);
        }
        else if (component is ITeamComponent teamComponent)
        {
            Team = teamComponent.Team;
            ControllerId = teamComponent.ControllerId;
        }
        else
        {
            if (_components.ContainsKey(type))
                throw new InvalidOperationException($"L'entitat {Id} ja té un component de tipus {type.Name}");

        }
        _components[type] = component;
    }

    public virtual void ReplaceComponent<T>(T component) where T : IGameEntityComponent
    {
        var type = component.GetType();
        if (component is ITeamComponent teamComponent)
        {
            Team = teamComponent.Team;
            ControllerId = teamComponent.ControllerId;
        }

        if (!_components.ContainsKey(type))
            AddComponent<T>(component);
        else
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

    public ITraitContainerComponent GetTraitComponent()
    {
        return GetRequiredComponent<ITraitContainerComponent>();
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
        return _components.ContainsKey(typeof(ITraitContainerComponent));
    }

    public bool HasTrait<T>() where T : IBaseTrait
    {
        return GetTraitComponent().HasTrait<T>();
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
