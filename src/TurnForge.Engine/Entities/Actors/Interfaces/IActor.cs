using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Actors.Interfaces;

public interface IActor : IGameEntity
{
    public HealthComponent HealthComponent { get; }

    public IBoardPositionId CurrentPosition
    {
        get;
    }

    public int CurrentHealth
    {
        get;
    }

}