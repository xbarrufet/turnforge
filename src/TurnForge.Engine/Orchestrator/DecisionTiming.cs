namespace TurnForge.Engine.Orchestrator;

public sealed record DecisionTiming(
    DecisionTimingWhen When,
    string? Phase,
    DecisionTimingFrequency Frequency)
{
    public static DecisionTiming Immediate => new(DecisionTimingWhen.OnCommandExecutionEnd, null, DecisionTimingFrequency.Single);
}

public enum DecisionTimingWhen
{
    OnStateStart,
    OnStateEnd,
    OnCommandExecutionEnd
}

public enum DecisionTimingFrequency
{
    Single,
    Permanent
}

