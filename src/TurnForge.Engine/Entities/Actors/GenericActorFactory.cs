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
        IEnumerable<ActorBehaviour>? extraBehaviours = null)
    {
        var def = gameCatalog.GetPropDefinition(typeId);

        // Convert definitions behaviors (assuming they are IActorBehaviour) to ActorBehaviour or BaseBehaviour
        // Need to ensure definitions store BaseBehaviour or convertible logic.
        // For now, assuming PropDefinition behaviours are compatible or need casting logic later.
        // But PropDefinition has IReadOnlyList<IActorBehaviour>.
        // BehaviourComponent expects IEnumerable<BaseBehaviour>.

        var behaviors = Merge(def.Behaviours, extraBehaviours);
        var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(behaviors.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>());

        return new Prop(
            EntityId.New(),
            def,
            new TurnForge.Engine.Entities.Components.PositionComponent(def.PositionComponentDefinition),
            component
        );
    }

    public Agent BuildAgent(
        AgentTypeId typeId,
        IEnumerable<ActorBehaviour>? extraBehaviours = null)
    {
        var def = gameCatalog.GetAgentDefinition(typeId);
        var behaviors = Merge(def.Behaviours, extraBehaviours);
        var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(behaviors.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>());

        return new Agent(
            EntityId.New(),
            def,
            new TurnForge.Engine.Entities.Components.PositionComponent(def.PositionComponentDefinition),
            component
        );
    }

    private static IEnumerable<IActorBehaviour> Merge(
        IReadOnlyList<IActorBehaviour> baseBehaviours,
        IEnumerable<ActorBehaviour>? extra)
    {
        if (extra == null)
            return baseBehaviours;

        return baseBehaviours.Concat(extra);
    }
}