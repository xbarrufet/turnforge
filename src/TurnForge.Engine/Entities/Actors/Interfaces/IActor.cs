using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Definitions.Actors.Interfaces;

public interface IActor : IGameEntity
{
    public IPositionComponent PositionComponent { get; }
    public IHealthComponent HealthComponent { get; }
    
    
}