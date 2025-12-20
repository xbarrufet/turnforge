using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Factories.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Interfaces;

public interface IActorFactory : IGameEntityFactory<Prop>, IGameEntityFactory<Agent>
{
    Prop BuildProp(PropDescriptor descriptor);
    Agent BuildAgent(AgentDescriptor descriptor);
}
