using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

// UPDATED
// For Prop, Agent

// For Zone

namespace TurnForge.Engine.Infrastructure.Factories.Interfaces;

public interface IGameEntityFactory
{
    Prop BuildProp(PropDescriptor descriptor);
    Agent BuildAgent(AgentDescriptor descriptor);
    Prop BuilProp(string definitionId, IBoardPositionId startPosition,IReadOnlyList<IGameEntityComponent>? components=null,  IReadOnlyList<ITrait>? traits=null);

    Agent BuildAgent(string definitionId, string teamId, string controllerId, IBoardPositionId startPosition,IReadOnlyList<IGameEntityComponent>? components=null,  IReadOnlyList<ITrait>? traits=null);

    Zone BuildZone(ZoneDescriptor descriptor);
    Connection BuildConnection(ConnectionDescriptor descriptor);
    
    


}
