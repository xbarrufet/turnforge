using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Components.Interfaces
{
    public interface IPositionComponent:IGameEntityComponent
    {
        public Position CurrentPosition { get; set;}
        public bool IsDiscrete {get;}
        public bool IsContinuous {get;}

        public static IPositionComponent Empty()
        {
            return new BasePositionComponent();
        }
    }
}