using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

/// <summary>
/// Factory for creating Action instances.
/// </summary>
public interface IActionFactory
{
    /// <summary>
    /// Build an action by its registered ID.
    /// </summary>
    IAction? BuildAction(ActionId actionId);
    
    /// <summary>
    /// Get all registered action IDs.
    /// </summary>
    IReadOnlyList<ActionId> GetRegisteredActionIds();
}