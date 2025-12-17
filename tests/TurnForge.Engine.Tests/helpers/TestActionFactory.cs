using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.helpers;

public sealed class TestActorFactory : IActorFactory
{
    public Prop BuildProp(PropDescriptor def, Position position)
    {
        return new Prop(def, position);
    }

    public Unit BuildUnit(UnitDescriptor def, Position position)
    {
        return new Unit(ActorId.New(), position, customType: def.CustomType);
    }

    public Hostile BuildHostile(HostileDescriptor def, Position position)
    {
        return new Hostile(ActorId.New(), position, customType: def.CustomType);
    }
}