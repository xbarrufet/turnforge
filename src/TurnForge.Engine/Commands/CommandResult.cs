namespace TurnForge.Engine.Commands;

public sealed record CommandResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; }

    public static CommandResult Ok(params string[] tags)
        => new() { Success = true, Tags = tags };

    public static CommandResult Fail(string error)
        => new() { Success = false, Error = error, Tags = Array.Empty<string>() };
}