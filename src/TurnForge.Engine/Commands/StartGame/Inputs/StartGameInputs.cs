using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board; // For Actor
using TurnForge.Engine.Entities.Board.Interfaces; // For IBoardPosition, IBoardDefinition
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action.Inputs;

/// <summary>
/// Input for adding a player with their agents to deploy.
/// </summary>
public record AddPlayerInput(
    PlayerId PlayerId,
    PlayerControllerType PlayerController,
    string Team,
    string PlayerName,
    string ActionPoolType,
    int MaxActions,
    List<AgentDeploymentInput> AgentDescriptors
) : IActionInput;

/// <summary>
/// Descriptor for a single agent to deploy.
/// Position is optional - if null, will be resolved by mission rules.
/// </summary>
public record AgentDeploymentInput(
    AgentDescriptor Descriptor,
    IBoardPosition? Position  // null = mission-based, value = explicit
);

public record PropDeploymentInput(
    PropDescriptor Descriptor,
    IBoardPosition? Position  // null = mission-based, value = explicit
);

public record ConfirmPlayersInput() : IActionInput;

public record MissionDataInput(
    string MissionName
) : IActionInput;

public record BoardDataInput(
    string MapId,
    BoardDescriptor BoardDescriptor,
    IReadOnlyList<ZoneDeployment> Zones,
    IReadOnlyList<ConnectionDeployment> Connections
) : IActionInput;
