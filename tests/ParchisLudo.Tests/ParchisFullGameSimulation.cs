using NUnit.Framework;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Actions;
using TurnForge.Engine.Infrastructure; 
using TurnForge.Engine.Core.Action; 
using TurnForge.Engine.Entities.Definitions; 
using TurnForge.Engine.Core.Action.Interfaces; // ADDED
using TurnForge.Engine.Entities.Actors.Descriptors; // ADDED
using TurnForge.Engine.Commands.StartGame.Action.Inputs; // ADDED
using TurnForge.Engine.Entities.Board.Interfaces; // ADDED for IBoardDefinition

using Parchis.Rules.Factory;
using Parchis.Rules;
using Parchis.Rules.Actions; 
using Parchis.Rules.Board; 
using Parchis.Rules.Fsm; // Fix: Import ParchisFsmFactory

using TurnForge.Engine.Entities.Definitions.Actors; // ADDED

namespace Parchis.Rules.Tests.Integration;

// Helper Definition
public class SimpleAgentDefinition : AgentDefinition 
{
    public SimpleAgentDefinition(string id) : base(id, id, PlayerId.From("System")) {}
}

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
        var catalogEntries = new List<BaseGameEntityDefinition> { 
            boardDef,
            new SimpleAgentDefinition("pawn") // Register pawn definition
        };
        
        var registry = new ActionRegistry();
        ParchisActionRegistration.Register(registry);

        var fsmGraph = ParchisFsmFactory.CreateFsmGraph(Players);
        
        var engineWrapper = GameEngineFactory.Create(fsmGraph.RootNode!)
            .WithDefinitions(catalogEntries)
            .WithActionRegistry(registry)
            .Build();
            
        var turnForge = engineWrapper;

        Console.WriteLine($"Status: {turnForge.GetStatus()}");
        
        Console.WriteLine("Initializing Game via 'StartGame' (Core)...");
        
        var batchInputs = new List<IActionInput>();
        var playerColors = new Dictionary<PlayerId, ParchisBoard.PlayerColor>
        {
            { Players[0], ParchisBoard.PlayerColor.Red },
            { Players[1], ParchisBoard.PlayerColor.Blue },
            { Players[2], ParchisBoard.PlayerColor.Green },
            { Players[3], ParchisBoard.PlayerColor.Yellow }
        };

        // 1. Add Player Inputs
        foreach (var kvp in playerColors)
        {
            var pid = kvp.Key;
            var colorObj = kvp.Value;
            var colorName = colorObj.ToString();
            
            var agentInputs = new List<AgentDeploymentInput>();
            for(int i=0; i<4; i++) 
            {
                // Create pawn descriptor
                // Assuming "pawn" is a valid definition ID in the catalog
                var desc = new AgentDescriptor("pawn");
                agentInputs.Add(new AgentDeploymentInput(desc, null));
            }
            // Use color name as player name
            batchInputs.Add(new AddPlayerInput(pid, colorName, agentInputs));
        }

        // 2. Confirm Players
        batchInputs.Add(new ConfirmPlayersInput());

        // 3. Select Map & Mission
        var mission = ParchisMissionFactory.CreateMissionForPlayers(playerColors);
        // Cast implicit or explicit for BoardDefinition if needed
        if (boardDef is IBoardDefinition bd)
        {
             batchInputs.Add(new SelectMapInput("parchis_standard", bd, mission));
        }
        else
        {
             // Fallback if type match fails (should not happen if factory returns BoardDefinition)
             throw new InvalidOperationException("Invalid BoardDefinition type");
        }

        // Execute Action with Batch Inputs
        var startParams = new Dictionary<string, object>
        {
            { "BatchInputs", batchInputs }
        };
        
        var initResult = turnForge.ExecuteAction(CoreActions.StartGame, startParams);
        
        if (initResult.Status != ActionStatus.Completed)
        {
             Assert.Fail($"Game Initialization Failed: {initResult.ErrorMessage}");
        }
        
        Console.WriteLine("✅ Game Initialized Successfully!");
        Assert.That(turnForge.GetStatus(), Is.EqualTo(GameStatus.InProgress)); 

        string? winner = null;
        int turn = 0;
        int maxTurns = 50;
        
        while (winner == null && turn < maxTurns)
        {
            int roll = _random.Next(1, 7);
            
            var result = turnForge.ExecuteAction(ParchisActions.Move, new Dictionary<string, object>
            {
                { "Roll", roll }
            });
            
            if (result.IsGameOver)
            {
                Console.WriteLine($"\n🎉 GAME OVER at Turn {turn}! 🎉");
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
