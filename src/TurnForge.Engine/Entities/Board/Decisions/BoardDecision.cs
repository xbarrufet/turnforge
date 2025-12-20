using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Decisions.Interfaces;

namespace TurnForge.Engine.Entities.Board.Decisions;

public sealed record BoardDecision(BoardDescriptor Board) : IBuildDecision<GameBoard>;
