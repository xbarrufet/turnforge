using TurnForge.Engine.Entities;
using TurnForge.Engine.Behaviours.Interfaces;

namespace TurnForge.Engine.Behaviours;

/// <summary>
/// Base class for all entity traits. Traits add dynamic logic to entities.
/// </summary>
public abstract class BaseTrait : IBaseTrait
{
    /// <summary>
    /// The entity that owns this behaviour
    /// </summary>
    public GameEntity Owner { get; internal set; } = null!;
}
