using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Commands.Interaction;

/// <summary>
/// Command to resume a suspended interaction.
/// Sent by UI after user provides input (dice roll, decision).
/// </summary>
public record SubmitInteractionCommand(
    Guid SessionId,
    Dictionary<string, object> InputData,
    bool Cancelled = false
) : ICommand
{
    public Type CommandType => typeof(SubmitInteractionCommand);
}
