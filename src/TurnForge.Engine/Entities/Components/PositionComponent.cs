using TurnForge.Engine.Entities.Components.Definitions;
using TurnForge.Engine.Entities.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Components
{
    public sealed class PositionComponent(PositionComponentDefinition definition) : IGameEntityComponent
    {
        // Dada pura: Coordenades en graella discreta o continua
        public PositionComponentDefinition Definition {get => definition;}
        public Position CurrentPosition { get; internal set; } = definition.Position;

        public bool IsDiscrete => CurrentPosition.IsDiscrete;
        public bool IsContinuous => CurrentPosition.IsContinuous;
        public static PositionComponent Empty => new PositionComponent(new PositionComponentDefinition(Position.Empty));
       
    }
}