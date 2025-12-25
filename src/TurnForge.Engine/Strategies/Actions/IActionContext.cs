using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;

namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Context providing raw access to game state and board for action strategies.
/// </summary>
/// <remarks>
/// Design Decision: Context only provides DATA access, not query helpers.
/// Query logic moved to IGameStateQuery service (shared with UI).
/// 
/// This separation allows:
/// - Strategies use IGameStateQuery for state queries
/// - Context remains simple (data holder only)
/// - UI and Backend share same query service
/// </remarks>
public interface IActionContext
{
    /// <summary>
    /// Current game state (immutable snapshot).
    /// </summary>
    GameState State { get; }
    
    /// <summary>
    /// Game board with spatial model for position validation.
    /// </summary>
    GameBoard Board { get; }
}
