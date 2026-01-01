using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Metrics;

/// <summary>
/// Interface for engine instrumentation metrics.
/// Compatible with OpenTelemetry via System.Diagnostics.Metrics.
/// </summary>
public interface IEngineMetrics
{
    /// <summary>
    /// Records the duration of a command execution.
    /// </summary>
    void RecordCommandDuration(string commandType, TimeSpan duration);

    /// <summary>
    /// Records the duration of a workflow execution (from start to completion).
    /// </summary>
    void RecordActionDuration(ActionId workflowId, TimeSpan duration);

    /// <summary>
    /// Increments the counter for a specific event type.
    /// </summary>
    void IncrementEventCount(string eventType);

    /// <summary>
    /// Increments the counter for a specific decision type.
    /// </summary>
    void IncrementDecisionCount(string decisionType);
}
