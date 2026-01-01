using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Interfaces;

public interface IGameEngine
{
    CommandTransaction ExecuteCommand(ICommand command);
    void SetFsmGraph(FsmGraph graph);
    
    /// <summary>
    /// Execute a workflow by its registered ID.
    /// Parameters are injected into the workflow context.
    /// </summary>
    ActionTransaction ExecuteAction(ActionId workflowId, Dictionary<string, object>? parameters = null);
    
    /// <summary>
    /// Get current game status.
    /// </summary>
    GameStatus GetStatus();
    
    /// <summary>
    /// Reset the game. Clears state, resets FSM to root, and returns to WaitingForStart.
    /// </summary>
    void ResetGame();
}
