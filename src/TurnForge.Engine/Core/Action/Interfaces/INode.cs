// ============================================================================
// TurnForge.Engine – Node Contract (Non-Generic)
// ============================================================================
//
// Design: Nodes receive the base ActionContext. Implementations that need
// typed access should cast internally. This keeps the orchestration simple.
// ============================================================================

using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

/// <summary>
/// A single step within an Action workflow.
/// </summary>
public interface INode
{
    /// <summary>
    /// Unique identifier for this node within the action.
    /// </summary>
    NodeId Id { get; }
    
    /// <summary>
    /// The next node in the chain, or null if this is the last node.
    /// </summary>
    INode? NextNode { get; set; }

    /// <summary>
    /// Execute this node's logic.
    /// </summary>
    /// <param name="context">The action's context (cast to specific type if needed)</param>
    /// <param name="state">Read-only view of the game state</param>
    /// <returns>Result indicating success, suspension, or failure</returns>
    ActionStepResult Execute(ActionContext context, GameStateView state);
}

/// <summary>
/// Extended node interface that supports programmatic linking.
/// Used by ActionBuilder to chain nodes together.
/// </summary>
public interface ILinkableNode : INode
{
    /// <summary>
    /// Set the next node in the chain.
    /// </summary>
    void SetNextNode(INode? next);
}