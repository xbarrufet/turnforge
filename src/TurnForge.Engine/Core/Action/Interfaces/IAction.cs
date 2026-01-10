// ============================================================================
// TurnForge.Engine – Action Contract (Non-Generic)
// ============================================================================
// 
// Design Decision: IAction is non-generic to simplify the orchestration layer.
// Type-safety for ActionContext is achieved by:
// 1. Each Action implementation defines its own typed context as a nested class or related class
// 2. Nodes that need typed access cast the context internally
// 3. The orchestrator only sees the base ActionContext
//
// This avoids type erasure issues while maintaining type-safety where it matters.
// ============================================================================

using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

/// <summary>
/// Represents a transactional resolution of a single command.
/// 
/// Characteristics:
/// - Closed and finite sequence of nodes
/// - Deterministic structure  
/// - No game rules inside
/// - Executed atomically by the orchestrator
/// </summary>
public interface IAction
{
    /// <summary>
    /// Identifier of the action type.
    /// Stable across executions.
    /// </summary>
    ActionId Id { get; }

    /// <summary>
    /// Entry point of the action execution.
    /// </summary>
    INode StartNode { get; }
    
    /// <summary>
    /// The context holding execution-scoped data.
    /// Implementations should provide a typed version internally.
    /// </summary>
    ActionContext Context { get; }

    /// <summary>
    /// Retrieves a node by its ID.
    /// Required for resuming suspended actions.
    /// </summary>
    INode GetNode(NodeId nodeId);

    /// <summary>
    /// Reactions that apply globally to the action.
    /// Evaluated at the end of the action execution loop.
    /// </summary>
    IReadOnlyCollection<IReaction> GlobalReactions { get; }
}
