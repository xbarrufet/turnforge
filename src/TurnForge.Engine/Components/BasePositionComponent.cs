using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Components
{
    public sealed class BasePositionComponent : IPositionComponent
    {
        public Position CurrentPosition { get; set; } = Position.Empty;

        public bool IsDiscrete => CurrentPosition.IsDiscrete;
        public bool IsContinuous => CurrentPosition.IsContinuous;

        public BasePositionComponent() { }

        public BasePositionComponent(Position position)
        {
            CurrentPosition = position;
        }

        public static BasePositionComponent Empty => new BasePositionComponent(Position.Empty);
    }
}