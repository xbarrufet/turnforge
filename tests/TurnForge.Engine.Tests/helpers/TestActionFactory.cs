using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.helpers;

public sealed class TestActionFactory : IActorFactory
{
    public Prop BuildProp(PropDescriptor descriptor) => Build(descriptor);

    public Agent BuildAgent(AgentDescriptor descriptor) => Build(descriptor);

    public Prop Build(IGameEntityDescriptor<Prop> descriptor)
    {
        var d = (PropDescriptor)descriptor;
        var behavioursList = d.ExtraBehaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
        var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(d.ExtraBehaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

        var pd = new PropDefinition(d.TypeId,
            new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(Position.Empty),
            new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(10),
            behavioursList
        );
        return new Prop(EntityId.New(), pd, new TurnForge.Engine.Entities.Components.PositionComponent(pd.PositionComponentDefinition), component);
    }

    public Agent Build(IGameEntityDescriptor<Agent> descriptor)
    {
        var d = (AgentDescriptor)descriptor;
        var behavioursList = d.ExtraBehaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
        var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(d.ExtraBehaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

        var ad = new AgentDefinition(d.TypeId,
            new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(Position.Empty),
            new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(10),
            new TurnForge.Engine.Entities.Components.Definitions.MovementComponentDefinition(3),
            behavioursList
        );
        return new Agent(EntityId.New(), ad, new TurnForge.Engine.Entities.Components.PositionComponent(ad.PositionComponentDefinition), component);
    }


}