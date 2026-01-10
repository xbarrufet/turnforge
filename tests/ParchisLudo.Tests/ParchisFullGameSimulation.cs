using Microsoft.Extensions.Logging; // ADDED: For ILogger
using NUnit.Framework;
using Parchis.Rules;

using TurnForge.Engine.Commands; // For LogLevel, LogContext
using TurnForge.Engine.Commands.StartGame;
using TurnForge.Engine.Commands.StartGame.Action.Inputs; // ADDED
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Core.Interfaces; // For IGameLogger
using TurnForge.Engine.Core.Logging;
// ADDED
using TurnForge.Engine.Entities.Board.Interfaces; // ADDED for IBoardDefinition
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Definitions.Board; // ADDED
using TurnForge.Engine.Entities.Players;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.Spawn;
using TurnForge.Engine.Extensions;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules.Tests.Integration;

/*
// Helper Definition


// Silent Logger to prevent OOM in long simulations
public class NullLogger : IGameLogger
{
    public void Log(TurnForge.Engine.Core.Logging.LogLevel level, string message, LogContext? context = null) { }
    public void LogError(string message, Exception? exception = null, LogContext? context = null) { }
}

// Console Logger for debugging
public class ConsoleGameLogger : IGameLogger
{
    public void Log(TurnForge.Engine.Core.Logging.LogLevel level, string message, LogContext? context = null)
    {
        // Plain output without prefixes or indentation
        Console.WriteLine(message);
    }

    public void LogError(string message, Exception? exception = null, LogContext? context = null)
    {
        Console.WriteLine(message);
        if (exception != null)
        {
            Console.WriteLine($"Exception: {exception.Message}");
        }
    }
}


[TestFixture]
public class ParchisFullGameSimulation
{
    private static readonly PlayerId[] Players = {
        PlayerId.From("red"),
        PlayerId.From("blue"),
        PlayerId.From("green"),
        PlayerId.From("yellow")
    };

    private static readonly int[] rolls = [5, 5, 5, 5, 6, 6, 6, 6, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 4, 4, 4, 4, 6, 6, 6];

    private static readonly Random _random = new(42);

    [Test]
    //[Explicit("Long running simulation")]
    public void Play_Until_GameOver_And_Reset()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("    PARCHIS FULL GAME SIMULATION (Cartridge Flow)");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var boardDef = ParchisBoardFactory.CreateDescriptor("parchis_standard");
        var catalogEntries = new List<BaseGameEntityDefinition> {
            boardDef,
            new PawnDefinition(PawnDefinition.DefId), // Register pawn definition,
            new SafetyZoneDefinition(SafetyZoneDefinition.DefId),
            new CenterZoneDefinition(CenterZoneDefinition.DefId),
            new SpawZoneDefinition(SpawZoneDefinition.DefId),
            new FinishLinitConnectionDefinition(FinishLinitConnectionDefinition.DefId) // Register connection definition
        };


        var fsmGraph = ParchisFsmFactory.CreateFsmGraph(Players);

        // Use ConsoleGameLogger to see debug output in console
        var engineWrapper = GameEngineFactory.Create(fsmGraph.RootNode!)
            .WithDefinitions(catalogEntries)
            .WithActionFactory(new ParchisActionFactory())
            .WithLogger(new ConsoleGameLogger()) // CHANGED: Use ConsoleGameLogger for debug output
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
            for (int i = 0; i < 4; i++)
            {
                // Create pawn descriptor
                // Assuming "pawn" is a valid definition ID in the catalog
                var desc = new AgentDescriptor(PawnDefinition.DefId, colorName,
                                colorName,
                                null,
                                [new ColorTrait(colorObj)]);
                agentInputs.Add(new AgentDeploymentInput(desc, ParchisBoard.GetSpawnPosition(colorObj)));
            }
            // Use color name as player name
            batchInputs.Add(new AddPlayerInput(pid, PlayerControllerType.AI, colorName, colorName, IActionPool.FixAmount, 1, agentInputs));
        }

        // 2. Confirm Players
        batchInputs.Add(new ConfirmPlayersInput());

        // 3. Select Map & Mission - Create board with zones and connections
        boardDef = ParchisBoardFactory.CreateDescriptor();
        var zones = ParchisZoneFactory.CreateZones();
        var connections = ParchisConnectionFactory.CreateConnections();

        var boardInput = new BoardDataInput(
            "parchis_standard",
            boardDef,
            zones,
            connections
        );
        batchInputs.Add(boardInput);



        // Execute Action with Typed Configuration (New Interface)
        var playerInputs = batchInputs.OfType<AddPlayerInput>().ToList();
        var boardDataInput = batchInputs.OfType<BoardDataInput>().FirstOrDefault();
        var missionDataInput = new MissionDataInput("parchis_standard");  // TODO: get mission name from mission
        var startParams = new StartGameParams(
            playerInputs,
            new List<PropDeploymentInput>(),  // TODO: populate props if needed
            boardDataInput!,
            missionDataInput
        );
        var initResult = GameEngineExtensions.ExecuteAction(turnForge, CoreActions.StartGameActionId, startParams);
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
            //int roll = _random.Next(1, 7);
            int roll = rolls[turn % rolls.Length];

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
*/
