using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities; // For GameEntity

namespace TurnForge.Engine.Traits.Interfaces;

/// <summary>
/// Base interface for all traits.
/// Traits are data containers attached to entities.
/// </summary>
public interface IDataTrait 
{
    /// <summary>
    /// The entity that owns this trait.
    /// </summary>
    GameEntity Owner { get; }
}

