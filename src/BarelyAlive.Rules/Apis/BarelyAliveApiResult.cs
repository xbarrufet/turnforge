using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace BarelyAlive.Rules.Apis;

public sealed record BarelyAliveApiResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; } = Array.Empty<string>();
    public IReadOnlyCollection<IDecision> Decisions { get; init; } = Array.Empty<IDecision>();

    public static BarelyAliveApiResult Ok(params string[] tags)
        => new() { Success = true, Tags = tags };

    public static BarelyAliveApiResult Fail(string error)
        => new() { Success = false, Error = error, Tags = Array.Empty<string>() };
}