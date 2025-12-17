using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Rules.BarelyAlive.Actors;

public class BarelyAliveActorFactory:IActorFactory
{
    
    public Hostile BuildHostile(
        HostileDescriptor hostileDescriptor,
        Position position)
        => BuildHostileInternal<Hostile>(
            hostileDescriptor, position);

    public Unit BuildUnit(
        UnitDescriptor unitDescriptor,
        Position position)
        => BuildUnitInternal<Unit>(
            unitDescriptor, position);

    public Prop BuildProp(
        PropDescriptor propDescriptor,
        Position position)
        => BuildPropInternal<Prop>(
            propDescriptor, position);
    public THostile BuildHostileInternal<THostile>(HostileDescriptor hostileDescriptor,Position position)
        where THostile : Hostile
    {
        return (THostile)Activator.CreateInstance(
            typeof(THostile),
            hostileDescriptor,
            position
        )!;
    }

    public TUnit BuildUnitInternal<TUnit>(UnitDescriptor unitDescriptor,Position position)
        where TUnit : Unit
    {
        return (TUnit)Activator.CreateInstance(
            typeof(TUnit),
            unitDescriptor,
            position
        )!;
    }

    public TProp BuildPropInternal<TProp>(PropDescriptor propDescriptor, Position position)
        where TProp : Prop
    {
        return (TProp)Activator.CreateInstance(
            typeof(TProp),
            propDescriptor,
            position
        )!;
    }
}