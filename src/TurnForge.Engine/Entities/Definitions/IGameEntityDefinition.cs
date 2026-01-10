using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions;

public interface IGameEntityDefinition
{
    public string DefinitionId { get; set; }
    public string Name { get; set; }
    public Category Category { get; set; }

    // ─────────────────────────────────────────────────────────────
    // Trait Management
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Adds a trait to the definition.
    /// If isRequired is true, the trait cannot be removed later.
    /// If the trait's StackAllowed is false and a trait of this type exists, it will be replaced.
    /// </summary>
    void AddTrait<TTrait>(TTrait trait, bool isRequired = false) where TTrait : class;

    /// <summary>
    /// Gets all traits of the specified type (required and non-required).
    /// </summary>
    IEnumerable<TTrait> GetTraits<TTrait>() where TTrait : ITrait;

    /// <summary>
    /// Gets only traits of the specified type that are marked as required.
    /// </summary>
    IEnumerable<TTrait> GetRequiredTraits<TTrait>() where TTrait : ITrait;

    /// <summary>
    /// Gets the first trait of the specified type, or null.
    /// </summary>
    TTrait? GetTrait<TTrait>() where TTrait : ITrait;

    /// <summary>
    /// Checks if any traits of the specified type exist.
    /// </summary>
    bool HasTrait<TTrait>() where TTrait : ITrait;

    /// <summary>
    /// Removes all traits of the specified type.
    /// Throws InvalidOperationException if the trait type is marked as required.
    /// </summary>
    void RemoveTrait<TTrait>() where TTrait : ITrait;
}