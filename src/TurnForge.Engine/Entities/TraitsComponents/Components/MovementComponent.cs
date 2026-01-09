using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.TraitsComponents.Components
{
    public sealed class PositionComponent
    {
        public int MaxUnitsToMove { get; set; }
        public IBoardPosition CurrentPosition { get; set; } = TilePosition.Empty;
        
        public PositionComponent() { }

        public PositionComponent(IBoardPosition position, int maxUnitsToMove = 1)
        {
            CurrentPosition = position;
            MaxUnitsToMove = maxUnitsToMove;
        }
        
        
        public static PositionComponent Empty => new PositionComponent();
    }
}