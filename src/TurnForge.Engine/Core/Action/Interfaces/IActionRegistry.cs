using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

/// <summary>
/// Registry for action factories.
/// Allows registering and retrieving action instances by ID.
/// </summary>
public interface IActionRegistry
{
    /// <summary>
    /// Register an action factory.
    /// </summary>
    void Register(ActionId id, Func<IAction> factory);
    
    /// <summary>
    /// Get an action instance by ID.
    /// Returns null if not registered.
    /// </summary>
    IAction? GetAction(ActionId id);
    
    /// <summary>
    /// Check if an action is registered.
    /// </summary>
    bool IsRegistered(ActionId id);
}
