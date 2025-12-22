using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Interfaces;

public interface IActor : IGameEntity
{
    public IPositionComponent PositionComponent { get; }
    public IHealthComponent HealthComponent { get; }
    
    
}