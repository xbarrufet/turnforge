using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Logging;

/// <summary>
/// Contextual information for structured log entries.
/// All properties are optional; include only what's relevant.
/// </summary>
public record LogContext
{
    public WorkflowExecutionId? ExecutionId { get; init; }
    public WorkflowId? WorkflowId { get; init; }
    public NodeId? NodeId { get; init; }
    public string? CommandType { get; init; }
    public IReadOnlyDictionary<string, string>? Tags { get; init; }

    public static LogContext ForWorkflow(WorkflowExecutionId executionId, WorkflowId? workflowId = null, NodeId? nodeId = null)
        => new() { ExecutionId = executionId, WorkflowId = workflowId, NodeId = nodeId };

    public static LogContext ForCommand(string commandType)
        => new() { CommandType = commandType };

    public override string ToString()
    {
        var parts = new List<string>();
        if (ExecutionId != null) parts.Add($"Exec:{ExecutionId.Value.ToString()[..8]}");
        if (WorkflowId != null) parts.Add($"Workflow:{WorkflowId}");
        if (NodeId != null) parts.Add($"Node:{NodeId}");
        if (CommandType != null) parts.Add($"Cmd:{CommandType}");
        return parts.Count > 0 ? $"[{string.Join("] [", parts)}]" : "";
    }
}
