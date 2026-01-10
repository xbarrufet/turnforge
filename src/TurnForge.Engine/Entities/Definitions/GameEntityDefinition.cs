using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions;

/// <summary>
/// Base definition for all game entities.
/// Definitions contain only Traits (configuration). Components (runtime state) are created by the Factory.
/// </summary>
public class BaseGameEntityDefinition : IGameEntityDefinition
{
    private readonly EntityItemCollection<ITrait> _traits = new();

    public string DefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Category Category { get; set; } = Category.Empty;

    // ─────────────────────────────────────────────────────────────
    // Constructors
    // ─────────────────────────────────────────────────────────────

    public BaseGameEntityDefinition() { }

    public BaseGameEntityDefinition(string definitionId)
    {
        DefinitionId = definitionId;
    }

    public BaseGameEntityDefinition(string definitionId, Category category)
    {
        DefinitionId = definitionId;
        Category = category;
    }

    // ─────────────────────────────────────────────────────────────
    // Trait Management
    // ─────────────────────────────────────────────────────────────

    public void AddTrait<TTrait>(TTrait trait, bool isRequired = false) where TTrait : class
    {
        if (trait is not ITrait iTrait)
            throw new ArgumentException($"Trait must implement ITrait interface", nameof(trait));

        var type = typeof(TTrait);
        _traits.AddWithStackValidation(type, iTrait, iTrait.StackAllowed, isRequired);
    }

    public IEnumerable<TTrait> GetTraits<TTrait>() where TTrait : ITrait
        => _traits.GetAll<TTrait>();

    public IEnumerable<TTrait> GetRequiredTraits<TTrait>() where TTrait : ITrait
        => _traits.GetAllRequired<TTrait>();

    public TTrait? GetTrait<TTrait>() where TTrait : ITrait
        => _traits.GetFirst<TTrait>();

    public bool HasTrait<TTrait>() where TTrait : ITrait
        => _traits.Has<TTrait>();

    public void RemoveTrait<TTrait>() where TTrait : ITrait
        => _traits.Remove(typeof(TTrait));

    /// <summary>
    /// All traits across all types.
    /// </summary>
    public IEnumerable<ITrait> Traits => _traits.GetAllItems();

    /// <summary>
    /// Gets the count of traits of the specified type.
    /// </summary>
    public int TraitCount<T>() where T : ITrait
        => _traits.Count<T>();

    // ─────────────────────────────────────────────────────────────
    // Legacy Fluent API (for backward compatibility)
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Adds a trait. Supports multiple traits of the same type if StackAllowed.
    /// </summary>
    public BaseGameEntityDefinition AddTrait(ITrait trait)
    {
        var type = trait.GetType();
        _traits.AddWithStackValidation(type, trait, trait.StackAllowed, isRequired: false);
        return this; // Fluent
    }

    /// <summary>
    /// Adds multiple traits.
    /// </summary>
    public BaseGameEntityDefinition AddTraits(params ITrait[] traits)
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
    public BaseGameEntityDefinition ReplaceTrait<T>(T trait) where T : ITrait
    {
        _traits.Remove(typeof(T));
        _traits.Add(typeof(T), trait, isRequired: false);
        return this;
    }

    /// <summary>
    /// Removes all traits of the specified type.
    /// </summary>
    public BaseGameEntityDefinition RemoveTraits<T>() where T : ITrait
    {
        _traits.Remove(typeof(T));
        return this;
    }
}