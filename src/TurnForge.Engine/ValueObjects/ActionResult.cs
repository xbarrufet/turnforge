using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.ValueObjects;

/// <summary>
    /// Final result of a workflow execution.
    /// This is structural only; no game semantics.
    /// </summary>
    public readonly record struct WorkflowResult(
        WorkflowExecutionId ExecutionId,
        WorkflowStatus Status
    );