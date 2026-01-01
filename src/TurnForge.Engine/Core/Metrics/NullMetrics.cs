using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Metrics;

/// <summary>
/// No-op implementation of IEngineMetrics for testing and default usage.
/// </summary>
public sealed class NullMetrics : IEngineMetrics
{
    public static readonly NullMetrics Instance = new();

    public void RecordCommandDuration(string commandType, TimeSpan duration) { }
    public void RecordActionDuration(ActionId workflowId, TimeSpan duration) { }
    public void IncrementEventCount(string eventType) { }
    public void IncrementDecisionCount(string decisionType) { }
}
