using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Factories.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Interfaces;

public interface IActorFactory
{
    Prop BuildProp(PropDescriptor descriptor);
    Agent BuildAgent(AgentDescriptor descriptor);
}
