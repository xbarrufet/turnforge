using TurnForge.Engine.Traits;
using TurnForge.Engine.Traits.Interfaces;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Components;

public class TraitContainerComponent : ITraitContainerComponent
{
    private List<BaseTrait> _traits = new();
    
    // AutoMapper needs a setter to inject definitions
    public IReadOnlyList<BaseTrait> Traits 
    { 
        get => _traits; 
        set => _traits = value.ToList(); 
    }

    public TraitContainerComponent(IEnumerable< BaseTrait> traits)
    {
        _traits = traits.ToList();
    }

    public TraitContainerComponent()
    {
        _traits = [];
    }

    public T? GetTrait<T>() where T : IBaseTrait
    {
        return _traits.OfType<T>().FirstOrDefault();
    }

    public bool HasTrait<T>() where T : IBaseTrait
    {
        return _traits.Any(b => b is T);
    }

    public T GetRequiredTrait<T>() where T : IBaseTrait
    {
        return _traits.OfType<T>().FirstOrDefault() ?? throw new InvalidOperationException($"Missed required trait {typeof(T).Name}");
    }

    public bool TryGetTrait<T>(out T? trait) where T : IBaseTrait
    {
        trait = GetTrait<T>();
        return trait != null;
    }
    
    public void AddTrait(IBaseTrait trait)
    {
        if (trait == null)
            throw new ArgumentNullException(nameof(trait));
        
        _traits.Add((BaseTrait)trait);
    }
    
    public bool RemoveTrait<T>() where T : IBaseTrait
    {
        var trait = _traits.OfType<T>().FirstOrDefault();
        if (trait is not BaseTrait baseTrait) return false;
        
        return _traits.Remove(baseTrait);
    }
    
    // Internal helper to set owner when attached
    internal void SetOwner(GameEntity owner)
    {
        foreach (var trait in _traits)
        {
            trait.Owner = owner;
        }
    }

    public static TraitContainerComponent Empty() => new();
}
