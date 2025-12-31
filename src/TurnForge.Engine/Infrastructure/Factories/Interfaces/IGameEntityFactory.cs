using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Definitions.Factories.Interfaces;
using TurnForge.Engine.Definitions.Board.Descriptors;
using TurnForge.Engine.Definitions.Board;

namespace TurnForge.Engine.Definitions.Factories.Interfaces;

public interface IGameEntityFactory
{
    Prop BuildProp(PropDescriptor descriptor);
    Agent BuildAgent(AgentDescriptor descriptor);
    TurnForge.Engine.Definitions.Board.Zone BuildZone(ZoneDescriptor descriptor);
}
