using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow;

public sealed class WorkflowExecutionResult
{
    public WorkflowStatus Status { get; }

    private WorkflowExecutionResult(WorkflowStatus status)
    {
        Status = status;
    }

    public static WorkflowExecutionResult Completed()
        => new(WorkflowStatus.Completed);

    public static WorkflowExecutionResult Cancelled()
        => new(WorkflowStatus.Cancelled);

    public static WorkflowExecutionResult Suspended()
        => new(WorkflowStatus.Suspended);
}

