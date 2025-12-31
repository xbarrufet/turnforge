using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components
{
    public sealed class BasePositionComponent : IPositionComponent
    {
        public IBoardPosition CurrentPosition { get; set; } = TilePosition.Empty;

        public bool IsDiscrete => CurrentPosition.Kind == BoardPositionKind.Tile;
        public bool IsContinuous => CurrentPosition.Kind == BoardPositionKind.Vector;

        public BasePositionComponent() { }

        public BasePositionComponent(IBoardPosition position)
        {
            CurrentPosition = position;
        }

        public BasePositionComponent(TurnForge.Engine.Traits.Standard.PositionTrait trait)
        {
            CurrentPosition = trait.InitialPosition;
        }

        public static BasePositionComponent Empty => new BasePositionComponent();
    }
}