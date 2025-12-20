using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public sealed class GenericActorFactory(
    IGameCatalog gameCatalog)
    : IActorFactory
{
    public Prop BuildProp(PropDescriptor descriptor)
    {
        var definition = gameCatalog.GetPropDefinition(descriptor.TypeId);
        var behaviours = descriptor.ExtraBehaviours ?? new List<IActorBehaviour>();

        var id = EntityId.New();
        var position = new PositionComponent(definition.PositionComponentDefinition);
        if (descriptor.Position != null)
        {
            position.CurrentPosition = descriptor.Position.Value;
        }

        var behaviourComponent = new BehaviourComponent(
            definition.Behaviours.Concat(behaviours).Cast<BaseBehaviour>()
        );

        return new Prop(id, definition, position, behaviourComponent);
    }

    public Agent BuildAgent(AgentDescriptor descriptor)
    {
        var definition = gameCatalog.GetAgentDefinition(descriptor.TypeId);
        var behaviours = descriptor.ExtraBehaviours ?? new List<IActorBehaviour>();

        var id = EntityId.New();
        var position = new PositionComponent(definition.PositionComponentDefinition);
        if (descriptor.Position != null)
        {
            position.CurrentPosition = descriptor.Position.Value;
        }

        var behaviourComponent = new BehaviourComponent(
            definition.Behaviours.Concat(behaviours).Cast<BaseBehaviour>()
        );

        return new Agent(id, definition, position, behaviourComponent);
    }

    // Explicit implementation for IGameEntityFactory<Prop>
    Prop IGameEntityFactory<Prop>.Build(IGameEntityDescriptor<Prop> descriptor)
    {
        return BuildProp((PropDescriptor)descriptor);
    }

    // Explicit implementation for IGameEntityFactory<Agent>
    Agent IGameEntityFactory<Agent>.Build(IGameEntityDescriptor<Agent> descriptor)
    {
        return BuildAgent((AgentDescriptor)descriptor);
    }
}