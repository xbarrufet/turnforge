using TurnForge.Engine.Entities.Appliers.Interfaces;

using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Commands;

public sealed record CommandResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public IReadOnlyCollection<IDecision> Decisions { get; init; } = [];
    public IReadOnlyCollection<string> Tags { get; init; } = [];

    public static CommandResult ACKResult => new() { Success = true, Tags = new[] { "ACK" } };

    public static CommandResult Ok(IDecision[] decisions, params string[] tags)
        => new() { Success = true, Decisions = decisions, Tags = tags };

    public static CommandResult Fail(string error)
        => new() { Success = false, Error = error };
}