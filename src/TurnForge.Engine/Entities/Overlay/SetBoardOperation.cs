using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

/// <summary>
/// Operation to set or replace the GameBoard at runtime.
/// Essential for initialization workflows (GameStart).
/// </summary>
public sealed class SetBoardOperation : IGameStateOperation
{
    public IGameBoard NewBoard { get; }
    public EntityId Target => EntityId.Empty; // Board is global, no specific entity target

    public SetBoardOperation(IGameBoard newBoard)
    {
        NewBoard = newBoard;
    }

    public void Apply(IGameStateMutator mutator)
    {
        mutator.SetBoard(NewBoard);
    }
}
