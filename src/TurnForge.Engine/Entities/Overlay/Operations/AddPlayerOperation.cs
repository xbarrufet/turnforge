using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

public record struct AddPlayerOperation(Player Player) : IGameStateOperation
{
    public EntityId EntityId => EntityId.Empty;
}
