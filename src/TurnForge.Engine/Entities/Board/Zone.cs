using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;


public sealed class Zone : GameEntity
{
    private readonly ZoneDefinition _definition;

    public Zone(
        ZoneDefinition definition,
        BehaviourComponent behaviourComponent) : base(definition.Id)
    {
        _definition = definition;
        AddComponent(behaviourComponent);
    }

    public bool Contains(Position position)
        => _definition.Bound.Contains(position);
}
