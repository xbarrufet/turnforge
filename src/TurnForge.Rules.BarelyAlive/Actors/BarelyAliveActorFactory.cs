using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Components;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Registration;

namespace TurnForge.Rules.BarelyAlive.Actors;

public sealed class BarelyAliveActorFactory : IActorFactory
{
    private readonly IDefinitionRegistry<NpcTypeId, NpcDefinition> _npcs;
    private readonly IDefinitionRegistry<UnitTypeId, UnitDefinition> _units;
    private readonly IDefinitionRegistry<PropTypeId, PropDefinition> _props;

    public BarelyAliveActorFactory(
        IDefinitionRegistry<NpcTypeId, NpcDefinition> npcs,
        IDefinitionRegistry<UnitTypeId, UnitDefinition> units,
        IDefinitionRegistry<PropTypeId, PropDefinition> props)
    {
        _npcs = npcs;
        _units = units;
        _props = props;
    }

    public Npc BuildNpc(
        NpcTypeId typeId,
        Position position,
        IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        var definition = _npcs.Get(typeId);

        var behaviours = definition.Behaviours;
        if (extraBehaviours != null)
        {
            behaviours = behaviours.Concat(extraBehaviours).ToList();
        }

        return new Npc(
            ActorId.New(),
            position,
            definition,
            behaviours
        );
    }

    public Unit BuildUnit(
        UnitTypeId typeId,
        Position position,
        IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        var definition = _units.Get(typeId);

        var behaviours = definition.Behaviours;
        if (extraBehaviours != null)
        {
            behaviours = behaviours.Concat(extraBehaviours).ToList();
        }

        return new Unit(
            ActorId.New(),
            position,
            definition,
            behaviours
        );
    }

    public Prop BuildProp(
        PropTypeId typeId,
        Position position,
        IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        var definition = _props.Get(typeId);

        HealthComponent? health =
            definition.MaxHealth is not null
                ? new HealthComponent(definition.MaxHealth.Value)
                : null;

        var behaviours = definition.Behaviours;
        if (extraBehaviours != null)
        {
            behaviours = behaviours.Concat(extraBehaviours).ToList();
        }

        return new Prop(
            ActorId.New(),
            position,
            definition,
            health,
            behaviours
        );
    }
}