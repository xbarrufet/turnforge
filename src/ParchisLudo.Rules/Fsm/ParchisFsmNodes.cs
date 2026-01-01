using Parchis.Rules.Fsm.Nodes;
using Parchis.Rules.Actions;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules.Fsm;

/// <summary>
/// Factory for creating Parchís FSM components.
/// 
/// Architecture (FSM 2.0):
/// - FSM: Handles turn order, phases, game end detection
/// - Actions: Atomic operations (Move)
/// - TurnOrderState: Dynamic player tracking in GameState
/// 
/// Graph structure (EndRound controls pattern):
/// StartRound → Turn → EndRound
///                      ↓ (if not IsRoundComplete)
///                   StartRound
///                      ↓ (if IsRoundComplete + winner)
///                   EndGame
/// </summary>
public static class ParchisFsmFactory
{
    /// <summary>
    /// Creates the complete Parchís FSM graph.
    /// Uses TurnOrderState for dynamic player tracking (supports 2-6 players).
    /// </summary>
    public static FsmGraph CreateFsmGraph(params PlayerId[] players)
    {
        // Create nodes
        var startRound = new ParchisStartRoundNode();
        var turn = new ParchisTurnNode();
        var endRound = new ParchisEndRoundNode();
        var endGame = new ParchisEndGameNode();
        
        // Wire up the graph:
        // StartRound → Turn → EndRound → StartRound (loop) or EndGame
        startRound.WithTurnNode(turn);
        turn.WithEndRound(endRound);
        endRound.WithStartRound(startRound).WithEndGame(endGame);
        
        // Build graph
        var builder = FsmBuilder.Create()
            .WithRoot(startRound)
            .WithNode(turn)
            .WithNode(endRound)
            .WithNode(endGame);
        
        return builder.Build();
    }
    
    /// <summary>
    /// Creates initial TurnOrderState from player list.
    /// </summary>
    public static TurnOrderState CreateTurnOrder(params PlayerId[] players)
    {
        return TurnOrderState.Create(players);
    }
    
    /// <summary>
    /// Creates the Move Action for Parchís.
    /// </summary>
    public static TurnForge.Engine.Core.Action.Interfaces.IAction CreateMoveAction()
    {
        return ParchisMoveActionFactory.Create();
    }
}

/// <summary>
/// End game node - terminal state when a player wins.
/// </summary>
public class ParchisEndGameNode : BaseFsmNode
{
    public ParchisEndGameNode() : base("EndGame") { }
    
    public override bool IsCompleted(GameState state) => false;  // Terminal
    public override BaseFsmNode? GetNextNode(GameState state) => null;  // No next
}
