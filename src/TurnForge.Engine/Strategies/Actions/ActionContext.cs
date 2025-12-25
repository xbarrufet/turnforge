using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;

namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Default implementation of IActionContext.
/// Simple data holder providing access to State and Board.
/// </summary>
public sealed class ActionContext : IActionContext
{
    public GameState State { get; }
    public GameBoard Board { get; }
    
    public ActionContext(GameState state, GameBoard board)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
        Board = board ?? throw new ArgumentNullException(nameof(board));
    }
}
