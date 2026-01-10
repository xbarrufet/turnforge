using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Nodes;

/// <summary>
/// Generic EndRound node for turn-based games.
/// 
/// Controls round completion:
/// - Checks if IsRoundComplete
/// - If not complete → StartRound (next player)
/// - If complete → EndGame or loop to new round
/// 
/// OnEntry: can advance turn order, check victory, etc.
/// </summary>
public class EndRoundNode : BaseFsmNode
{
    private BaseFsmNode? _startRoundNode;
    private BaseFsmNode? _endGameNode;
    
    public EndRoundNode(
        // add the NextPlayerAction in OnEntry to advance turn order
        ) : base("EndRound") { }
    
    /// <summary>
    /// Configure where to go for next player's turn.
    /// </summary>
    public EndRoundNode WithStartRound(BaseFsmNode startRound)
    {
        _startRoundNode = startRound;
        return this;
    }
    
    /// <summary>
    /// Configure where to go when game ends.
    /// </summary>
    public EndRoundNode WithEndGame(BaseFsmNode endGame)
    {
        _endGameNode = endGame;
        return this;
    }

   
    public override bool IsCompleted(GameStateView state)
    {
        // Always complete immediately
        return true;
    }
    
    public override BaseFsmNode? GetNextNode(GameStateView state)
    {
        // Check if all players have played this round
        if (state.TurnOrder.IsRoundComplete)
        {
            // Check for winner (override in subclass for game-specific logic)
            if (CheckGameOver(state))
            {
                return _endGameNode;
            }
            
            // Start a new round (will reset turn order via workflow)
            return _startRoundNode;
        }
        
        // Not all players done, continue to next player
        return _startRoundNode;
    }
    
    /// <summary>
    /// Override in subclass to check for game over condition.
    /// </summary>
    protected virtual bool CheckGameOver(GameStateView state)
    {
        return false; // Default: game never ends
    }
    
    protected BaseFsmNode? StartRoundNode => _startRoundNode;
    protected BaseFsmNode? EndGameNode => _endGameNode;
    
   
    
}
