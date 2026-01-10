using System.Diagnostics;
using TurnForge.Engine.Definitions.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

[DebuggerDisplay("{GetType().Name,nq}: {Name} (ID: {Id})")]
public abstract class GameEntity : IGameEntity, IComponentContainer
{
    public EntityId Id { get; }
    public string DefinitionId { get; }
    public string Name { get; set; } = string.Empty;
    public Category Category { get; set; } = Category.Empty;
  
    protected GameEntity(EntityId id, string name, Category category, string definitionId)
    {
        Id = id;
        Name = name;
        Category = category;
        DefinitionId = definitionId;
    }

    // Traits: Configuration/Definition (values immutable, but can add/remove traits)
    private Dictionary<Type, ITrait> _traits = new();

    // Components: Runtime state (mutable, based on traits)
    private Dictionary<Type, IGameEntityComponent> _components = new();

    public virtual GameEntity Clone()
    {
        var clone = (GameEntity)this.MemberwiseClone();
        clone._traits = new Dictionary<Type, ITrait>(this._traits);
        clone._components = new Dictionary<Type, IGameEntityComponent>(this._components);
        return clone;
    }

    public IReadOnlyCollection<IGameEntityComponent> Components => _components.Values;


    public void  AddComponent<T>(T component) where T : IGameEntityComponent
    {
        var type = component.GetType();
        // replace in case it already exists
        /*if (_components.ContainsKey(type))
                throw new InvalidOperationException($"L'entitat {Id} ja té un component de tipus {type.Name}");*/
        _components[type] = component;
    }

    public void ReplaceComponent<T>(T component) where T : IGameEntityComponent
    {
        var type = component.GetType();
        if (!_components.ContainsKey(type))
            AddComponent<T>(component);
        else
            _components[type] = component;
    }

    public virtual bool RemoveComponent<T>() where T : IGameEntityComponent
    {
        return _components.Remove(typeof(T));
    }

    public bool HasComponent<T>() where T : class, IGameEntityComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    public T? GetComponent<T>() where T : class, IGameEntityComponent
    {
        return GetComponent(typeof(T)) as T;
    }



    public T GetRequiredComponent<T>() where T : class, IGameEntityComponent
    {
        if(!TryGetComponent<T>(out var component))
            throw new InvalidOperationException($"Entity {Id} missing required component {typeof(T).Name}");
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

    private IGameEntityComponent? GetComponent(Type componentType)
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
        return true; // No required components by default
    }

    // ========== Trait Management ==========

    /// <summary>
    /// Gets a trait of the specified type.
    /// </summary>
    public T? GetTrait<T>() where T : class, ITrait
    {
        return _traits.TryGetValue(typeof(T), out var trait) ? trait as T : null;
    }

    /// <summary>
    /// Checks if the entity has a trait of the specified type.
    /// </summary>
    public bool HasTrait<T>() where T : class, ITrait
    {
        return _traits.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Adds a trait to the entity and sets its owner.
    /// </summary>
    public void AddTrait<T>(T trait) where T : class, ITrait
    {
        if (trait == null)
            throw new ArgumentNullException(nameof(trait));
        // Use runtime type, not compile-time generic type
        var type = trait.GetType();
        // replace in case it already exists
        _traits[type] = trait;
    }

    /// <summary>
    /// Removes a trait of the specified type.
    /// </summary>
    public bool RemoveTrait<T>() where T : class, ITrait
    {
        return _traits.Remove(typeof(T));
    }

    /// <summary>
    /// Gets all traits attached to this entity.
    /// </summary>
    public IEnumerable<ITrait> GetAllTraits()
    {
        return _traits.Values;
    }

    public T GetRequiredTrait<T>() where T : class, ITrait
    {
        if(!TryGetTrait<T>(out var trait))
            throw new InvalidOperationException($"Entity {Id} missing required trait {typeof(T).Name}");
        return trait!;
    }

    public string GetComponents()
    {
        return string.Join(", ", _components.Values.Select(c => c.GetType().Name));
    }

    public IEnumerable<IGameEntityComponent> GetAllComponents()
    {
        return _components.Values;
    }

    /// <summary>
    /// Tries to get a trait of the specified type.
    /// </summary>
    public bool TryGetTrait<T>(out T? trait) where T : class, ITrait
    {
        trait = GetTrait<T>();
        return trait != null;
    }

    public static GameEntity Emtpy => new EmptyGameEntity();
    protected static Category SystemCategory = new Category("SystemCategory");
}


public class EmptyGameEntity() : GameEntity(EntityId.Empty, "Empty Entity",SystemCategory, "empty_definition");