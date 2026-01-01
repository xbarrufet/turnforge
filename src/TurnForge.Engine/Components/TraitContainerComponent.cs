using TurnForge.Engine.Traits;
using TurnForge.Engine.Traits.Interfaces;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities; // For GameEntity

namespace TurnForge.Engine.Components;

public class TraitContainerComponent : ITraitContainerComponent
{
    private List<BaseDataTrait> _traits = new();
    
    // AutoMapper needs a setter to inject definitions
    public IReadOnlyList<BaseDataTrait> Traits 
    { 
        get => _traits; 
        set => _traits = value.ToList(); 
    }

    public TraitContainerComponent(IEnumerable< BaseDataTrait> traits)
    {
        _traits = traits.ToList();
    }

    public TraitContainerComponent()
    {
        _traits = [];
    }

    public T? GetTrait<T>() where T : IDataTrait
    {
        return _traits.OfType<T>().FirstOrDefault();
    }

    public bool HasTrait<T>() where T : IDataTrait
    {
        return _traits.Any(b => b is T);
    }

    public T GetRequiredTrait<T>() where T : IDataTrait
    {
        return _traits.OfType<T>().FirstOrDefault() ?? throw new InvalidOperationException($"Missed required trait {typeof(T).Name}");
    }

    public bool TryGetTrait<T>(out T? trait) where T : IDataTrait
    {
        trait = GetTrait<T>();
        return trait != null;
    }
    
    public void AddTrait(IDataTrait trait)
    {
        if (trait == null)
            throw new ArgumentNullException(nameof(trait));
        
        _traits.Add((BaseDataTrait)trait);
    }
    
    public bool RemoveTrait<T>() where T : IDataTrait
    {
        var trait = _traits.OfType<T>().FirstOrDefault();
        if (trait is not BaseDataTrait baseTrait) return false;
        
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
