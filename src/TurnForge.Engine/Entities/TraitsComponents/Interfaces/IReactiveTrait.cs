using TurnForge.Engine.Core.Action.Interfaces;

namespace TurnForge.Engine.Entities.TraitsComponents.Interfaces;

/// <summary>
/// Interface for traits that react to workflow events.
/// A reactive trait combines:
/// - Data (trait properties)
/// - Trigger (which event type activates it)
/// - Reaction logic (via IReaction interface)
/// 
/// Example: ExplosiveTrait triggers on MovedToEvent and deals damage.
/// </summary>
public interface IReactiveTrait : ITrait, IReaction
{
    /// <summary>
    /// The event type that triggers this trait's reaction.
    /// When an event of this type occurs, the system will check
    /// if this trait's CanReact returns true.
    /// </summary>
    Type TriggerEvent { get; }
}
