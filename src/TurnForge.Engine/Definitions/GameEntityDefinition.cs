using TurnForge.Engine.Traits.Interfaces;
using TurnForge.Engine.Traits.Standard;

namespace TurnForge.Engine.Definitions;

/// <summary>
/// Base definition for all game entities.
/// Uses Dictionary with List to support multiple traits of the same type.
/// Provides O(1) trait lookup by type.
/// </summary>
public class BaseGameEntityDefinition
{
    private readonly Dictionary<Type, List<IBaseTrait>> _traitsByType = new();
    
    public string DefinitionId { get; set; } = string.Empty;
    
    public BaseGameEntityDefinition() { }
    
    public BaseGameEntityDefinition(string definitionId)
    {
        DefinitionId = definitionId;
    }
    
    public BaseGameEntityDefinition(string definitionId, string category)
    {
        DefinitionId = definitionId;
        AddTrait(new IdentityTrait(category));
    }
    
    // ─────────────────────────────────────────────────────────────
    // Trait Access (O(1) by Type)
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// All traits across all types.
    /// </summary>
    public IEnumerable<IBaseTrait> Traits => _traitsByType.Values.SelectMany(x => x);
    
    /// <summary>
    /// Gets ALL traits of the specified type.
    /// </summary>
    public IEnumerable<T> GetTraits<T>() where T : IBaseTrait 
        => _traitsByType.TryGetValue(typeof(T), out var list) 
            ? list.Cast<T>() 
            : Enumerable.Empty<T>();
    
    /// <summary>
    /// Gets the FIRST trait of the specified type, or null.
    /// </summary>
    public T? GetTrait<T>() where T : IBaseTrait 
        => GetTraits<T>().FirstOrDefault();
    
    /// <summary>
    /// Checks if this definition has at least one trait of the specified type.
    /// </summary>
    public bool HasTrait<T>() where T : IBaseTrait 
        => _traitsByType.ContainsKey(typeof(T));
    
    /// <summary>
    /// Gets the count of traits of the specified type.
    /// </summary>
    public int TraitCount<T>() where T : IBaseTrait
        => _traitsByType.TryGetValue(typeof(T), out var list) ? list.Count : 0;
    
    // ─────────────────────────────────────────────────────────────
    // Trait Modification
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Adds a trait. Supports multiple traits of the same type.
    /// </summary>
    public BaseGameEntityDefinition AddTrait(IBaseTrait trait)
    {
        var type = trait.GetType();
        if (!_traitsByType.TryGetValue(type, out var list))
        {
            list = new List<IBaseTrait>();
            _traitsByType[type] = list;
        }
        list.Add(trait);
        return this; // Fluent
    }
    
    /// <summary>
    /// Adds multiple traits.
    /// </summary>
    public BaseGameEntityDefinition AddTraits(params IBaseTrait[] traits)
    {
        foreach (var trait in traits)
        {
            AddTrait(trait);
        }
        return this;
    }
    
    /// <summary>
    /// Replaces ALL traits of the given type with the new trait.
    /// </summary>
    public BaseGameEntityDefinition ReplaceTrait<T>(T trait) where T : IBaseTrait
    {
        _traitsByType[typeof(T)] = new List<IBaseTrait> { trait };
        return this;
    }
    
    /// <summary>
    /// Removes all traits of the specified type.
    /// </summary>
    public BaseGameEntityDefinition RemoveTraits<T>() where T : IBaseTrait
    {
        _traitsByType.Remove(typeof(T));
        return this;
    }
}