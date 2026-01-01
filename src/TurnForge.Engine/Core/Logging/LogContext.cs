using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Logging;

/// <summary>
/// Contextual information for structured log entries.
/// All properties are optional; include only what's relevant.
/// </summary>
public record LogContext
{
    public ActionExecutionId? ExecutionId { get; init; }
    public ActionId? ActionId { get; init; }
    public NodeId? NodeId { get; init; }
    public string? CommandType { get; init; }
    public IReadOnlyDictionary<string, string>? Tags { get; init; }

    public static LogContext ForAction(ActionExecutionId executionId, ActionId? workflowId = null, NodeId? nodeId = null)
        => new() { ExecutionId = executionId, ActionId = workflowId, NodeId = nodeId };

    public static LogContext ForCommand(string commandType)
        => new() { CommandType = commandType };

    public override string ToString()
    {
        var parts = new List<string>();
        if (ExecutionId != null) parts.Add($"Exec:{ExecutionId.Value.ToString()[..8]}");
        if (ActionId != null) parts.Add($"Action:{ActionId}");
        if (NodeId != null) parts.Add($"Node:{NodeId}");
        if (CommandType != null) parts.Add($"Cmd:{CommandType}");
        return parts.Count > 0 ? $"[{string.Join("] [", parts)}]" : "";
    }
}
