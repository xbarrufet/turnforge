using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Traits.Interfaces;

namespace TurnForge.Engine.Entities.TraitsComponents.Components;

public class PositionableTrait(IBoardPosition position): ITrait
{
    public IBoardPosition Position { get; } = position;
}