using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.helpers;

public sealed class TestActionFactory : IActorFactory
{
    public Prop BuildProp(PropTypeId typeId, Position position, IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        return new Prop(new ActorId(System.Guid.NewGuid()), position, new PropDefinition(typeId, 0, 0, extraBehaviours ?? new List<IActorBehaviour>(), 10), null, extraBehaviours);
    }

    public Agent BuildAgent(AgentTypeId typeId, Position position, IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        return new Agent(new ActorId(System.Guid.NewGuid()), position, new AgentDefinition(typeId, 10, 3, 2, extraBehaviours ?? new List<IActorBehaviour>()), extraBehaviours);
    }
}