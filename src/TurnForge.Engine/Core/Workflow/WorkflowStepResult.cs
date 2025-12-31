using System;
using System.Collections.Generic;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Workflow.Interfaces;

namespace TurnForge.Engine.Core.Workflow;

/// <summary>
/// Rich result object returned by Workflow Nodes.
/// Handles flow control (Next, Suspend) and carries interaction data.
/// </summary>
public sealed class WorkflowStepResult
{
    public WorkflowStatus Status { get; }
    
    // For Suspended state
    public string? Reason { get; private set; }
    public IReadOnlyList<Type>? AllowedInputTypes { get; private set; }
    
    // For Failed state
    public string? ErrorMessage { get; private set; }

    private WorkflowStepResult(WorkflowStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Node execution finished. Orchestrator should move to NextNode.
    /// </summary>
    public static WorkflowStepResult Success() 
        => new(WorkflowStatus.Completed);

    /// <summary>
    /// Node execution blocked. Needs external input.
    /// </summary>
    /// <param name="reason">Human-readable reason for UI ("Waiting for Player Selection")</param>
    /// <param name="allowedInputs">List of expected IWorkflowInput types</param>
    public static WorkflowStepResult Suspend(string reason, params Type[] allowedInputs)
    {
        return new WorkflowStepResult(WorkflowStatus.Suspended)
        {
            Reason = reason,
            AllowedInputTypes = allowedInputs
        };
    }

    /// <summary>
    /// Node execution encountered a blocking error.
    /// </summary>
    public static WorkflowStepResult Fail(string error) 
        => new(WorkflowStatus.Failed) { ErrorMessage = error };
}
