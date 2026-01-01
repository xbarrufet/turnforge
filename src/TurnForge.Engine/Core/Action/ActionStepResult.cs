using System;
using System.Collections.Generic;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Action.Interfaces;

namespace TurnForge.Engine.Core.Action;

/// <summary>
/// Rich result object returned by Action Nodes.
/// Handles flow control (Next, Suspend) and carries interaction data.
/// </summary>
public sealed class ActionStepResult
{
    public ActionStatus Status { get; }
    
    // For Suspended state
    public string? Reason { get; private set; }
    public IReadOnlyList<Type>? AllowedInputTypes { get; private set; }
    
    // For Failed state
    public string? ErrorMessage { get; private set; }

    private ActionStepResult(ActionStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Node execution finished. Orchestrator should move to NextNode.
    /// </summary>
    public static ActionStepResult Success() 
        => new(ActionStatus.Completed);

    /// <summary>
    /// Node execution blocked. Needs external input.
    /// </summary>
    /// <param name="reason">Human-readable reason for UI ("Waiting for Player Selection")</param>
    /// <param name="allowedInputs">List of expected IActionInput types</param>
    public static ActionStepResult Suspend(string reason, params Type[] allowedInputs)
    {
        return new ActionStepResult(ActionStatus.Suspended)
        {
            Reason = reason,
            AllowedInputTypes = allowedInputs
        };
    }

    /// <summary>
    /// Node execution encountered a blocking error.
    /// </summary>
    public static ActionStepResult Fail(string error) 
        => new(ActionStatus.Failed) { ErrorMessage = error };
}
