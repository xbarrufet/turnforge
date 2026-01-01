using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Traits.Interfaces;

namespace TurnForge.Engine.Traits;

/// <summary>
/// Base class for all entity traits. Traits add dynamic logic to entities.
/// </summary>
#pragma warning disable CS0618 // IBaseTrait is obsolete - we use it here for backwards compatibility
public abstract class BaseDataTrait : IBaseTrait
#pragma warning restore CS0618
{
    /// <summary>
    /// The entity that owns this behaviour
    /// </summary>
    public GameEntity Owner { get; internal set; } = null!;
}

