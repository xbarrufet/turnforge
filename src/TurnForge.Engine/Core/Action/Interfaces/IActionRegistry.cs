using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

/// <summary>
/// Registry for workflows that can be executed by ID.
/// Games register their workflows here during bootstrap.
/// </summary>
public interface IActionRegistry
{
    /// <summary>
    /// Register a workflow factory. Factory is called each time workflow is executed.
    /// </summary>
    void Register(ActionId id, Func<IAction> factory);
    
    /// <summary>
    /// Get a new instance of a workflow by ID.
    /// </summary>
    IAction? GetAction(ActionId id);
    
    /// <summary>
    /// Check if a workflow is registered.
    /// </summary>
    bool IsRegistered(ActionId id);
}
