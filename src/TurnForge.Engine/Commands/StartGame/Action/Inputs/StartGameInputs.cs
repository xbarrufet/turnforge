using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Commands.StartGame.Workflow.Inputs;

/// <summary>
/// Input for adding a player with their agents to deploy.
/// </summary>
public record AddPlayerInput(
    PlayerId PlayerId,
    string PlayerName,
    List<AgentDeploymentInput> AgentDescriptors
) : IWorkflowInput;

/// <summary>
/// Descriptor for a single agent to deploy.
/// Position is optional - if null, will be resolved by mission rules.
/// </summary>
public record AgentDeploymentInput(
    IGameEntityBuildDescriptor Descriptor,
    IBoardPosition? Position  // null = mission-based, value = explicit
);

public record ConfirmPlayersInput() : IWorkflowInput;

public record SelectMapInput(
    string MapId,
    IBoardDefinition BoardDefinition,
    MissionData MissionData
) : IWorkflowInput;
