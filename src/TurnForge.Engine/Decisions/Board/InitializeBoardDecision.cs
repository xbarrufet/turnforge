using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Decisions.Board;

/// <summary>
/// Decision to add the initialized GameBoard to the game state.
/// Contains the fully constructed board with zones.
/// </summary>
public sealed record InitializeBoardDecision(GameBoard Board) : IDecision
{
    public DecisionTiming Timing { get; init; } = DecisionTiming.Immediate;
    public string OriginId { get; init; } = "System";
}
