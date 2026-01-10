using System.Collections.Immutable;
using NUnit.Framework;
using Parchis.Rules.Fsm;
using Parchis.Rules.Fsm.Nodes;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Tests;

[TestFixture]
public class TurnOrderFlowTests
{
    [Test]
    public void TurnOrder_AllPlayersGetTurn_ThenEndRound()
    {
        // Arrange: Create 3 players
        var player1 = PlayerId.From("red");
        var player2 = PlayerId.From("blue");
        var player3 = PlayerId.From("green");

        // Create initial turn order starting at index 0
        var turnOrder = TurnOrderState.Create(new[] { player1, player2, player3 });

        // Create initial state with turn order
        var builder = new GameStateBuilder(GameState.Empty());
        builder.SetTurnOrder(turnOrder);
        var state = builder.Build();

        // Create FSM graph
        var startRound = new ParchisStartRoundNode();
        var turn = new ParchisTurnNode();
        var endRound = new ParchisEndRoundNode();
        var endGame = new ParchisEndGameNode();

        // Wire: StartRound → Turn → EndRound → StartRound (or EndGame)
        startRound.WithTurnNode(turn);
        turn.WithEndRound(endRound);
        endRound.WithStartRound(startRound).WithEndGame(endGame);

        var graph = FsmBuilder.Create()
            .WithRoot(startRound)
            .WithNode(turn)
            .WithNode(endRound)
            .WithNode(endGame)
            .Build();

        // Act: Simulate the flow
        var currentState = state;
        var playersPlayed = new List<PlayerId>();

        // === Player 1's turn ===
        Assert.That(currentState.TurnOrder.CurrentPlayer, Is.EqualTo(player1));

        // ProcessFlow: StartRound → Turn (immediate transition)
        var result = graph.ProcessFlow(currentState);
        currentState = result.State;
        Assert.That(graph.CurrentNode, Is.EqualTo(turn));
        playersPlayed.Add(player1);


        // ProcessFlow: Turn → EndRound → StartRound (EndRound sees not complete)
        var builder1 = new GameStateBuilder(currentState);
        builder1.SetTurnOrder(currentState.TurnOrder.NextPlayer());
        currentState = builder1.Build();

        result = graph.ProcessFlow(currentState);
        currentState = result.State;

        // === Player 2's turn ===
        Assert.That(currentState.TurnOrder.CurrentPlayer, Is.EqualTo(player2));
        Assert.That(graph.CurrentNode, Is.EqualTo(turn));
        playersPlayed.Add(player2);


        var builder2 = new GameStateBuilder(currentState);
        builder2.SetTurnOrder(currentState.TurnOrder.NextPlayer());
        currentState = builder2.Build();

        result = graph.ProcessFlow(currentState);
        currentState = result.State;

        // === Player 3's turn ===
        Assert.That(currentState.TurnOrder.CurrentPlayer, Is.EqualTo(player3));
        Assert.That(graph.CurrentNode, Is.EqualTo(turn));
        playersPlayed.Add(player3);


        // Advance past last player
        var builder3 = new GameStateBuilder(currentState);
        builder3.SetTurnOrder(currentState.TurnOrder.NextPlayer());
        currentState = builder3.Build();

        // Now IsRoundComplete should be true
        Assert.That(currentState.TurnOrder.IsRoundComplete, Is.True);

        result = graph.ProcessFlow(currentState);
        currentState = result.State;

        // EndRound saw IsRoundComplete, but no winner so loops back to StartRound
        // StartRound auto-transitions to Turn
        Assert.That(graph.CurrentNode, Is.EqualTo(turn));

        // Verify all 3 players played
        Assert.That(playersPlayed.Count, Is.EqualTo(3));
        Assert.That(playersPlayed.Select(p => p.Value), Does.Contain("red"));
        Assert.That(playersPlayed.Select(p => p.Value), Does.Contain("blue"));
        Assert.That(playersPlayed.Select(p => p.Value), Does.Contain("green"));
    }


}
