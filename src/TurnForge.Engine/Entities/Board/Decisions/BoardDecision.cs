using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;

using TurnForge.Engine.Orchestrator;

namespace TurnForge.Engine.Entities.Board.Decisions;

public sealed record BoardDecision(BoardDescriptor Board) : IBuildDecision<GameBoard>
{
    public DecisionTiming Timing { get; init; } = DecisionTiming.Immediate;
    public string OriginId { get; init; } = "System";
    public IGameEntityDescriptor<GameBoard> Descriptor => Board;
}
