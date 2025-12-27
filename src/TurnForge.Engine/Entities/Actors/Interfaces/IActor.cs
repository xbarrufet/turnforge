using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Definitions.Actors.Interfaces;

public interface IActor : IGameEntity
{
    public IPositionComponent PositionComponent { get; }
    public IHealthComponent HealthComponent { get; }
    
    
}