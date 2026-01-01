using NUnit.Framework;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core;
using TurnForge.Engine.Infrastructure; 
using TurnForge.Engine.Core.Action; 
using TurnForge.Engine.Entities.Definitions; 
using Parchis.Rules.Factory;
using Parchis.Rules;
using Parchis.Rules.Actions; 
using Parchis.Rules.Board; 
using Parchis.Rules.Fsm; // Fix: Import ParchisFsmFactory

namespace Parchis.Rules.Tests.Integration;

[TestFixture]
public class ParchisFullGameSimulation
{
    private static readonly PlayerId[] Players = { 
        PlayerId.From("RED"), 
        PlayerId.From("BLUE"), 
        PlayerId.From("GREEN"), 
        PlayerId.From("YELLOW") 
    };
    
    private static readonly Random _random = new(42);

    [Test]
    [Explicit("Long running simulation")]
    public void Play_Until_GameOver_And_Reset()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("    PARCHIS FULL GAME SIMULATION (Cartridge Flow)");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var boardDef = ParchisBoardFactory.CreateDescriptor("parchis_standard");
        var catalogEntries = new List<BaseGameEntityDefinition> { boardDef };
        
        var registry = new ActionRegistry();
        ParchisActionRegistration.Register(registry);

        var fsmGraph = ParchisFsmFactory.CreateFsmGraph(Players);
        
        var engineWrapper = GameEngineFactory.Create(fsmGraph.RootNode!)
            .WithDefinitions(catalogEntries)
            .WithActionRegistry(registry)
            .Build();
            
        var turnForge = engineWrapper;

        Console.WriteLine($"Status: {turnForge.GetStatus()}");
        
        Console.WriteLine("Initializing Game via 'parchis_game_start'...");
        var startParams = new Dictionary<string, object>
        {
            { "BoardId", "parchis_standard" },
            { "PlayerIds", Players.Select(p => p.Value).ToList() }
        };
        
        var initResult = turnForge.ExecuteAction(ParchisActions.StartGame, startParams);
        
        if (initResult.Status != ActionStatus.Completed)
        {
             Assert.Fail($"Game Initialization Failed: {initResult.ErrorMessage}");
        }
        
        Console.WriteLine("✅ Game Initialized Successfully!");
        Assert.That(turnForge.GetStatus(), Is.EqualTo(GameStatus.InProgress)); 

        string? winner = null;
        int turn = 0;
        int maxTurns = 5000;
        
        while (winner == null && turn < maxTurns)
        {
            var currentPlayer = Players[turn % 4]; 
            int roll = _random.Next(1, 7);
            
            var turnNode = fsmGraph.GetNode(new NodeId("Turn")) as Parchis.Rules.Fsm.Nodes.ParchisTurnNode;
            if (turnNode != null)
            {
                turnNode.ConsumeAction(roll == 6);
            }

            var result = turnForge.ExecuteAction(ParchisActions.Move, new Dictionary<string, object>
            {
                { "Roll", roll },
                { "PlayerId", currentPlayer }
            });
            
            if (result.IsGameOver)
            {
                Console.WriteLine($"\n🎉 GAME OVER at Turn {turn}! 🎉");
                winner = currentPlayer.Value;
                break;
            }

            turn++;
        }
        
        if (winner != null)
        {
            Console.WriteLine($"Winner: {winner}");
            turnForge.ResetGame();
            Assert.That(turnForge.GetStatus(), Is.EqualTo(GameStatus.WaitingForStart));
            Console.WriteLine("✅ Reset successful!");
        }
        else
        {
             Console.WriteLine($"⚠️ Max turns reached.");
        }
    }
}
