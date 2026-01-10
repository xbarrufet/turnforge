using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

/// <summary>
/// Operation to spend action points from a player.
/// Used by actions (like Move) to consume AP via the overlay.
/// </summary>
public record struct SpendAPOperation(PlayerId PlayerId,EntityId EntityId, int amouunt=1) : IGameStateOperation
{   
}
