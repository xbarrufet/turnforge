using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Definitions.Factories.Interfaces;

namespace TurnForge.Engine.Definitions.Actors.Interfaces;

public interface IActorFactory
{
    Prop BuildProp(PropDescriptor descriptor);
    Agent BuildAgent(AgentDescriptor descriptor);
}
