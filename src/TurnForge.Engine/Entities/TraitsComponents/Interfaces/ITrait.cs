

// For GameEntity

namespace TurnForge.Engine.Entities.TraitsComponents.Interfaces;

/// <summary>
/// Base interface for all traits.
/// Traits are data containers attached to entities.
/// </summary>
public interface ITrait
{
    /// <summary>
    /// Indicates whether this trait was initialized with a parametric constructor.
    /// Returns false if created with default constructor, true if created with parameters.
    /// This helps distinguish between placeholder traits and fully configured traits.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Indicates whether multiple instances of this trait type can exist on the same entity.
    /// Default is false - most traits should not stack (e.g., only one VitalityTrait per entity).
    /// Override to true for traits that can legitimately stack (e.g., multiple status effects).
    /// </summary>
    bool StackAllowed { get; }
}

