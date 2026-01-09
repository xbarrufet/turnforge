using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;


namespace TurnForge.Engine.Entities.Actors.Descriptors;

public class AgentDescriptor : GameEntityBuildDescriptor
{

    public AgentDescriptor(string definitionId,
                                    string teamId,
                                    PlayerId playerId,
                                 IEnumerable<IGameEntityComponent>? extraComponents = null,
                                 IEnumerable<ITrait>? requestedTraits = null)
                                 : base(
                                    definitionId,
                                    extraComponents,
                                    requestedTraits)
    {
        // created the team component

    }

}

