// ============================================================================
// TurnForge.Engine – Workflow Contract
// ============================================================================

using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow.Interfaces;

public interface IWorkflow
{
    /// <summary>
    /// A Workflow represents the transactional resolution of a single command.
    ///
    /// Characteristics:
    /// - Closed and finite sequence of nodes
    /// - Deterministic structure
    /// - No game rules inside
    /// - Executed atomically by the orchestrator
    /// </summary>

        /// <summary>
        /// Identifier of the workflow type.
        /// Stable across executions.
        /// </summary>
        WorkflowId Id { get; }

        /// <summary>
        /// Entry point of the workflow execution.
        /// </summary>
        INode StartNode { get; }

        /// <summary>
        /// Retrieves a node by its ID.
        /// Required for resuming suspended workflows.
        /// </summary>
        /// <summary>
        /// Retrieves a node by its ID.
        /// Required for resuming suspended workflows.
        /// </summary>
        INode GetNode(NodeId nodeId);

        /// <summary>
        /// Reactions that apply globally to the workflow, usually triggered by events.
        /// Evaluated at the end of the workflow execution loop.
        /// </summary>
        IReadOnlyCollection<IReaction> GlobalReactions { get; }
    }

