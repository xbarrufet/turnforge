using TurnForge.Engine.Behaviours;
using TurnForge.Engine.Behaviours.Interfaces;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Components;

public class BaseBehaviourComponent : IBehaviourComponent
{
    private List<BaseBehaviour> _behaviours = new();
    
    // AutoMapper needs a setter to inject definitions
    public IReadOnlyList<BaseBehaviour> Behaviours 
    { 
        get => _behaviours; 
        set => _behaviours = value.ToList(); 
    }

    public BaseBehaviourComponent(IEnumerable<BaseBehaviour> behaviours)
    {
        _behaviours = behaviours.ToList();
    }

    public BaseBehaviourComponent()
    {
        _behaviours = [];
    }

    public T? GetBehaviour<T>() where T : IBaseBehaviour
    {
        return _behaviours.OfType<T>().FirstOrDefault();
    }

    public bool HasBehaviour<T>() where T : IBaseBehaviour
    {
        return _behaviours.Any(b => b is T);
    }

    public T GetRequiredBehaviour<T>() where T : IBaseBehaviour
    {
        return _behaviours.OfType<T>().FirstOrDefault() ?? throw new InvalidOperationException($"Missed required behaviour {typeof(T).Name}");
    }

    public bool TryGetBehaviour<T>(out T? behaviour) where T : IBaseBehaviour
    {
        behaviour = GetBehaviour<T>();
        return behaviour != null;
    }
    
    public void AddBehaviour(IBaseBehaviour behaviour)
    {
        if (behaviour == null)
            throw new ArgumentNullException(nameof(behaviour));
        
        _behaviours.Add((BaseBehaviour)behaviour);
    }
    
    public bool RemoveBehaviour<T>() where T : IBaseBehaviour
    {
        var behaviour = _behaviours.OfType<T>().FirstOrDefault();
        if (behaviour is not BaseBehaviour baseBehaviour) return false;
        
        return _behaviours.Remove(baseBehaviour);
    }
    
    // Internal helper to set owner when attached
    internal void SetOwner(GameEntity owner)
    {
        foreach (var behaviour in _behaviours)
        {
            behaviour.Owner = owner;
        }
    }

    public static BaseBehaviourComponent Empty() => new();
}
