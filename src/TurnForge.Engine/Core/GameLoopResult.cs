namespace TurnForge.Engine.Core;

public sealed class GameLoopResult
{
    public bool IsAllowed { get; }
    public bool RequiresAck { get; }
    public object? DomainResult { get; }
    public string? Reason { get; }

    private GameLoopResult(
        bool isAllowed,
        bool requiresAck,
        object? domainResult,
        string? reason)
    {
        IsAllowed = isAllowed;
        RequiresAck = requiresAck;
        DomainResult = domainResult;
        Reason = reason;
    }

    public static GameLoopResult Allowed(
        bool requiresAck = false,
        object? domainResult = null)
        => new(true, requiresAck, domainResult, null);

    public static GameLoopResult Rejected(string reason)
        => new(false, false, null, reason);
}