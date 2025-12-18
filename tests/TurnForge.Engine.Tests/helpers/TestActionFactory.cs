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
        return new Prop(new ActorId(System.Guid.NewGuid()), position, new PropDefinition(typeId, null, extraBehaviours ?? new List<IActorBehaviour>()), null, extraBehaviours);
    }

    public Unit BuildUnit(UnitTypeId typeId, Position position, IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        return new Unit(new ActorId(System.Guid.NewGuid()), position, new UnitDefinition(typeId, 10, extraBehaviours ?? new List<IActorBehaviour>()), extraBehaviours);
    }

    public Npc BuildNpc(NpcTypeId typeId, Position position, IReadOnlyList<IActorBehaviour>? extraBehaviours = null)
    {
        return new Npc(new ActorId(System.Guid.NewGuid()), position, new NpcDefinition(typeId, 10, 2, extraBehaviours ?? new List<IActorBehaviour>()), extraBehaviours);
    }
}