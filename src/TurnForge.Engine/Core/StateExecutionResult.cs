namespace TurnForge.Engine.Core;

public enum StateResultKind
{
    Completed,
    Rejected
}

public sealed class StateExecutionResult
{
    public StateResultKind Kind { get; }
    public object? DomainResult { get; }
    public bool RequiresAck { get; }

    private StateExecutionResult(
        StateResultKind kind,
        object? domainResult,
        bool requiresAck)
    {
        Kind = kind;
        DomainResult = domainResult;
        RequiresAck = requiresAck;
    }

    // --------- FACTORIES ---------

    public static StateExecutionResult Completed(
        object? result = null,
        bool requiresAck = false)
        => new(StateResultKind.Completed, result, requiresAck);

    public static StateExecutionResult Rejected(
        object? reason = null)
        => new(StateResultKind.Rejected, reason, false);
}
