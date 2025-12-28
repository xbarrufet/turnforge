using System.Diagnostics.Metrics;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Metrics;

/// <summary>
/// OpenTelemetry-compatible metrics implementation using System.Diagnostics.Metrics.
/// Requires an external MeterListener or OTel SDK to export metrics.
/// </summary>
public sealed class OpenTelemetryMetrics : IEngineMetrics
{
    private static readonly Meter Meter = new("TurnForge.Engine", "1.0.0");

    private static readonly Histogram<double> CommandDurationHistogram = 
        Meter.CreateHistogram<double>("turnforge.command.duration_ms", "ms", "Duration of command execution");

    private static readonly Histogram<double> WorkflowDurationHistogram = 
        Meter.CreateHistogram<double>("turnforge.workflow.duration_ms", "ms", "Duration of workflow execution");

    private static readonly Counter<long> EventCounter = 
        Meter.CreateCounter<long>("turnforge.events.count", "events", "Count of workflow events");

    private static readonly Counter<long> DecisionCounter = 
        Meter.CreateCounter<long>("turnforge.decisions.count", "decisions", "Count of recorded decisions");

    public void RecordCommandDuration(string commandType, TimeSpan duration)
    {
        CommandDurationHistogram.Record(duration.TotalMilliseconds, new KeyValuePair<string, object?>("command_type", commandType));
    }

    public void RecordWorkflowDuration(WorkflowId workflowId, TimeSpan duration)
    {
        WorkflowDurationHistogram.Record(duration.TotalMilliseconds, new KeyValuePair<string, object?>("workflow_id", workflowId.ToString()));
    }

    public void IncrementEventCount(string eventType)
    {
        EventCounter.Add(1, new KeyValuePair<string, object?>("event_type", eventType));
    }

    public void IncrementDecisionCount(string decisionType)
    {
        DecisionCounter.Add(1, new KeyValuePair<string, object?>("decision_type", decisionType));
    }
}
