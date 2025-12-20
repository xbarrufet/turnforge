using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Entities.Actors.Interfaces;

using TurnForge.Engine.ValueObjects;

public interface IActorFactory
{
    Prop BuildProp(PropTypeId typeId, IEnumerable<ActorBehaviour>? extraBehaviours = null);
    Agent BuildAgent(AgentTypeId typeId, IEnumerable<ActorBehaviour>? extraBehaviours = null);
}
