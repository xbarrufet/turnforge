using TurnForge.Engine.Entities.Actors.Components;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Registration;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class GenericActorFactory(
    IGameCatalog gameCatalog)
    : IActorFactory
{
    public Prop BuildProp(
        PropTypeId typeId,
        Position position,
        IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        var def = gameCatalog.GetPropDefinition(typeId);

        return new Prop(
            ActorId.New(),
            position,
            def,
            def.MaxHealth>0? new HealthComponent(def.MaxHealth) : null,
            Merge(def.Behaviours, extraBehaviours)
        );
    }

    public Unit BuildUnit(
        UnitTypeId typeId,
        Position position,
        IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        var def = gameCatalog.GetUnitDefinition(typeId);

        return new Unit(
            ActorId.New(),
            position,
            def,
            Merge(def.Behaviours, extraBehaviours)
        );
    }

    public Npc BuildNpc(
        NpcTypeId typeId,
        Position position,
        IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        var def = gameCatalog.GetNpcDefinition(typeId);

        return new Npc(
            ActorId.New(),
            position,
            def,
            Merge(def.Behaviours, extraBehaviours)
        );
    }

    private static IReadOnlyList<IActorBehaviour> Merge(
        IReadOnlyList<IActorBehaviour> baseBehaviours,
        IReadOnlyList<IActorBehaviour>? extra)
    {
        if (extra == null || extra.Count == 0)
            return baseBehaviours;

        return baseBehaviours.Concat(extra).ToList();
    }
    
   
}