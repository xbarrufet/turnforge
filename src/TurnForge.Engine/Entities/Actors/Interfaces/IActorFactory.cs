using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Entities.Actors.Interfaces;

using TurnForge.Engine.ValueObjects;

public interface IActorFactory
{
    Hostile BuildHostile(
        HostileDescriptor hostileDescriptor,
        Position position
    );

    Unit BuildUnit(
        UnitDescriptor unitDescriptor,
        Position position
    );

    Prop BuildProp(
        PropDescriptor propDescriptor,
        Position position
    );
}