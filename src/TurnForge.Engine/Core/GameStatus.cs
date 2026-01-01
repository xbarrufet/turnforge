namespace TurnForge.Engine.Core;

/// <summary>
/// Current status of the game engine.
/// </summary>
public enum GameStatus
{
    /// <summary>
    /// Engine is ready but no game has started yet.
    /// Waiting for StartGame workflow.
    /// </summary>
    WaitingForStart,
    
    /// <summary>
    /// Game is in progress.
    /// </summary>
    InProgress,
    
    /// <summary>
    /// Game has ended. Call ResetGame() to start a new game.
    /// </summary>
    GameOver
}
