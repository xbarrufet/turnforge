namespace TurnForge.Engine.Entities.TraitsComponents.Interfaces;

/// <summary>
/// Base interface for all entity components.
/// Components represent state and behavior that can be attached to game entities.
/// </summary>
public interface IGameEntityComponent
{
    /// <summary>
    /// Indicates whether this component was created using its parameterized constructor
    /// with specific initialization values, as opposed to a default/empty constructor.
    /// </summary>
    bool IsInitialized { get; }
}