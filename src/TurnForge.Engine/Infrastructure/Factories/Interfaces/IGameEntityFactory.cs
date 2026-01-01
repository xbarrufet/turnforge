using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Definitions.Factories.Interfaces;
using TurnForge.Engine.Entities.Board.Descriptors; // UPDATED
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Actors; // For Prop, Agent
using TurnForge.Engine.Entities.Board; // For Zone

namespace TurnForge.Engine.Definitions.Factories.Interfaces;

public interface IGameEntityFactory
{
    Prop BuildProp(PropDescriptor descriptor);
    Agent BuildAgent(AgentDescriptor descriptor);
    TurnForge.Engine.Entities.Board.Zone BuildZone(ZoneDescriptor descriptor);
}
