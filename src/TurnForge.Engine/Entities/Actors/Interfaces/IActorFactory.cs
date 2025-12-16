using TurnForge.Engine.Commands.Game.Definitions;

namespace TurnForge.Engine.Entities.Actors.Interfaces;

using TurnForge.Engine.ValueObjects;

public interface IActorFactory
{
    Actor CreateActor(ActorDefinition definition);
}