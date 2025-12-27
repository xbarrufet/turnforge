using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components
{
    public sealed class BasePositionComponent : IPositionComponent
    {
        public Position CurrentPosition { get; set; } = Position.Empty;

        public bool IsDiscrete => CurrentPosition.IsTile;
        public bool IsContinuous => CurrentPosition.IsVector;

        public BasePositionComponent() { }

        public BasePositionComponent(Position position)
        {
            CurrentPosition = position;
        }

        public BasePositionComponent(TurnForge.Engine.Traits.Standard.PositionTrait trait)
        {
            CurrentPosition = trait.InitialPosition;
        }

        public static BasePositionComponent Empty => new BasePositionComponent(Position.Empty);
    }
}