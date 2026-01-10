using TurnForge.Engine.Commands.StartGame.Action.Inputs;
using TurnForge.Engine.Core.Action.Interfaces;

namespace TurnForge.Engine.Commands.StartGame;

/// <summary>
/// Strongly-typed parameters for StartGame action.
/// 1. Player Inputs-> incldues player data and AgentDeployments
/// 2. Board Inputs -> includes BoardDefinition, Zones and Connections
/// 3. Prop Inputs -> includes PropDeployments
/// 4. Mission Data -> All data about mission: objectives,etc.
/// </summary>
public record StartGameParams(
    List<AddPlayerInput> PlayerInputs,
    List<PropDeploymentInput> PropInputs,
    BoardDataInput BoardData,
    MissionDataInput MissionData
) : IActionParameters;
