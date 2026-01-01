using System;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities; // For GameEntity

namespace TurnForge.Engine.Components.Board;

public sealed class ZoneBoundComponent : IZoneBoundComponent
{
    public GameEntity Entity { get; set; } = null!;
    public IZoneBound Bound { get; }

    public ZoneBoundComponent(IZoneBound bound)
    {
        Bound = bound ?? throw new ArgumentNullException(nameof(bound));
    }

    public void OnAttached(GameEntity entity) 
    {
        Entity = entity;
    }
    
    public void OnDetached()
    {
        Entity = null!;
    }
}

public interface IZoneBoundComponent : IGameEntityComponent
{
    IZoneBound Bound { get; }
}
