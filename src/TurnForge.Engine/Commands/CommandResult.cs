using TurnForge.Engine.Appliers.Entity.Interfaces;

using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Commands;

public sealed record CommandResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public IReadOnlyCollection<IDecision> Decisions { get; init; } = [];
    public IReadOnlyCollection<string> Tags { get; init; } = [];

    /// <summary>
    /// Request for user interaction (only present if Suspended).
    /// </summary>
    public Strategies.Interactions.InteractionRequest? Interaction { get; init; }
    
    /// <summary>
    /// Whether the command execution is suspended waiting for input.
    /// </summary>
    public bool IsSuspended => Interaction != null;

    public static CommandResult ACKResult => new() { Success = true, Tags = new[] { "ACK" } };

    public static CommandResult Ok(IDecision[] decisions, params string[] tags)
        => new() { Success = true, Decisions = decisions, Tags = tags };

    public static CommandResult Fail(string error)
        => new() { Success = false, Error = error };
        
    public static CommandResult Suspended(Strategies.Interactions.InteractionRequest request)
        => new() { Success = true, Interaction = request, Tags = new[] { "SUSPENDED" } };
}