namespace TurnForge.Engine.Core;

public sealed record CommandResponseEnvelope(string CommandName, object? Payload);

