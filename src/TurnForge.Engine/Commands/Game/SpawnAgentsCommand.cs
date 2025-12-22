using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.Spawn;

namespace TurnForge.Engine.Commands.Game;

/// <summary>
/// Command to spawn agents in the game.
/// Uses the new spawn pipeline with SpawnRequests.
/// </summary>
/// <param name="Requests">List of spawn requests containing definition IDs and configuration</param>
public sealed record SpawnAgentsCommand(
    IReadOnlyList<SpawnRequest> Requests
) : ICommand
{
    public Type CommandType => typeof(SpawnAgentsCommand);
}