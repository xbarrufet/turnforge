using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions.CoreBase;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

public class AgentDescriptor : GameEntityBuildDescriptor
{
    // Direct properties for Agent-specific data
    public TeamId Team { get; init; }
    public PlayerId Controller { get; init; }
    public IBoardPositionId StartPosition { get; init; }

    public AgentDescriptor(
        string name,
        TeamId teamId,
        PlayerId playerId,
        IBoardPositionId startPosition,
        string definitionId = BasicAgentDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? requestedTraits = null)
        : base(
            definitionId: definitionId,
            name: name,
            extraComponents: extraComponents,
            definitionTraitValues: requestedTraits)
    {
        // MembershipTrait and MovementComponent removed - Team, Controller, StartPosition are direct properties
        Team = teamId;
        Controller = playerId;
        StartPosition = startPosition;
    }

    public AgentDescriptor(
        TeamId teamId,
        PlayerId playerId,
        IBoardPositionId startPosition,
        string definitionId = BasicAgentDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? requestedTraits = null)
        : base(
            definitionId,
            definitionId + "_" + playerId,
            extraComponents,
            requestedTraits)
    {
        // MembershipTrait and MovementComponent removed - Team, Controller, StartPosition are direct properties
        Team = teamId;
        Controller = playerId;
        StartPosition = startPosition;
    }

    public AgentDescriptor(
        string name,
        TeamId teamId,
        PlayerId playerId,
        string definitionId = BasicAgentDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? requestedTraits = null)
        : base(
            definitionId: definitionId,
            name: name,
            extraComponents: extraComponents,
            definitionTraitValues: requestedTraits)
    {
        // MembershipTrait and MovementComponent removed - Team, Controller, StartPosition are direct properties
        Team = teamId;
        Controller = playerId;
        StartPosition = IBoardPositionId.Limbo;
    }

    public AgentDescriptor(
        TeamId teamId,
        PlayerId playerId,
        string definitionId = BasicAgentDefinition.DefinitionId,
        IEnumerable<IGameEntityComponent>? extraComponents = null,
        IEnumerable<ITrait>? requestedTraits = null)
        : base(
            definitionId,
            definitionId + "_" + playerId,
            extraComponents,
            requestedTraits)
    {
        // MembershipTrait and MovementComponent removed - Team, Controller, StartPosition are direct properties
        Team = teamId;
        Controller = playerId;
        StartPosition = IBoardPositionId.Limbo;
    }
}
