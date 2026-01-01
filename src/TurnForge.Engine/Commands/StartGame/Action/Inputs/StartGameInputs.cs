using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Actors; // For Actor
using TurnForge.Engine.Entities.Board.Interfaces; // For IBoardPosition, IBoardDefinition
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action.Inputs;

/// <summary>
/// Input for adding a player with their agents to deploy.
/// </summary>
public record AddPlayerInput(
    PlayerId PlayerId,
    string PlayerName,
    List<AgentDeploymentInput> AgentDescriptors
) : IActionInput;

/// <summary>
/// Descriptor for a single agent to deploy.
/// Position is optional - if null, will be resolved by mission rules.
/// </summary>
public record AgentDeploymentInput(
    IGameEntityBuildDescriptor Descriptor,
    IBoardPosition? Position  // null = mission-based, value = explicit
);

public record ConfirmPlayersInput() : IActionInput;

public record SelectMapInput(
    string MapId,
    IBoardDefinition BoardDefinition,
    MissionDefinition? Mission
) : IActionInput;
