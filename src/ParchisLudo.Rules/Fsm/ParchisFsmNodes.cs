using ParchisLudo.Rules.Actions;
using ParchisLudo.Rules.Fsm.Nodes;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Players.ValueObjects;

namespace ParchisLudo.Rules.Fsm;

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
    /// 
    /// New architecture with EndTurnNode:
    /// RoundNode → TurnNode → EndTurnNode → [TurnNode (next player) | EndRoundNode]
    ///                                                                      ↓
    ///                                                          [EndGameNode | RoundNode]
    /// </summary>
    public static FsmGraph CreateFsmGraph(params PlayerId[] players)
    {
        // Create nodes
        var startRound = new ParchisStartRoundNode();
        var turn = new ParchisTurnNode();
        var endTurn = new TurnForge.Engine.Core.Fsm.Nodes.EndTurnNode();
        var endRound = new ParchisEndRoundNode();
        var endGame = new ParchisEndGameNode();

        // Wire up the graph:
        // RoundNode (reset AP) → TurnNode → EndTurnNode (advance player) → [TurnNode | EndRoundNode]
        startRound.WithTurnNode(turn);

        turn.WithEndRound(endTurn);  // Turn completes → EndTurn

        endTurn
            .WithTurnNode(turn)      // Next player's turn
            .WithEndRound(endRound); // All players done → EndRound

        endRound
            .WithStartRound(startRound)  // New round
            .WithEndGame(endGame);       // Game over

        // Build graph
        var builder = FsmBuilder.Create()
            .WithRoot(startRound)
            .WithNode(turn)
            .WithNode(endTurn)
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

    // TODO: Fix - ParchisMoveAction does not have Create method
}

/// <summary>
/// End game node - terminal state when a player wins.
/// </summary>
public class ParchisEndGameNode : BaseFsmNode
{
    public ParchisEndGameNode() : base("EndGame") { }

    public override bool IsCompleted(GameStateView state) => false;  // Terminal
    public override BaseFsmNode? GetNextNode(GameStateView state) => null;  // No next
}
