using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Entities.Actors.Interfaces;

using TurnForge.Engine.ValueObjects;

public interface IActorFactory
{
    Prop BuildProp(PropTypeId typeId, Position position, IReadOnlyList<IActorBehaviour>? extraBehaviours = null);
    Agent BuildAgent(AgentTypeId typeId, Position position, IReadOnlyList<IActorBehaviour>? extraBehaviours = null);
}
