using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;


public sealed class Zone : GameEntity
{
    public IZoneBound Bound { get; }

    public Zone(
        EntityId id,
        IZoneBound bound,
        BehaviourComponent behaviourComponent) : base(id)
    {
        Bound = bound;
        AddComponent(behaviourComponent);
    }

    public bool Contains(Position position)
        => Bound.Contains(position);
}
