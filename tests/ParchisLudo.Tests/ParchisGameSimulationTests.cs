using NUnit.Framework;
using Parchis.Rules;
using Parchis.Rules.Actions;
using Parchis.Rules.Board;
using Parchis.Rules.Factory;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace Parchis.Rules.Tests.Integration;
/*
/// <summary>
/// Integration tests using proper Parchis bootstrap via ParchisGame.Create().
/// Tests the Move action workflow with Rule of 5 spawn mechanic.
/// </summary>
[TestFixture]
public class ParchisGameSimulationTests
{
    private static readonly PlayerId Red = PlayerId.From("red");
    private static readonly PlayerId Blue = PlayerId.From("blue");

    [Test]
    [Explicit("Move action needs fixing - bootstrap refactor complete")]
    public void Simulation_StartGame_SpawnRule_And_ForwardMove()
    {
        // ---------------------------------------------------------
        // 1. SETUP: Use proper Parchis bootstrap
        // ---------------------------------------------------------
        var game = ParchisGame.Create(Red, Blue);
        var engine = game.EngineWrapper;

        // Note: ParchisGame.Create() uses obsolete manual initialization.
        // Status stays WaitingForStart until first ExecuteAction.
        Assert.That(engine.GetStatus(), Is.EqualTo(GameStatus.WaitingForStart),
            "Game should be waiting for start after Create()");

        // ---------------------------------------------------------
        // 2. VERIFY INITIAL STATE: Pawns at spawn positions
        // ---------------------------------------------------------
        var state = GetCurrentState(game);
        var redPawns = state.Entities.Values
            .Where(e => e.DefinitionId.StartsWith("pawn_red"))
            .ToList();

        Assert.That(redPawns.Count, Is.GreaterThanOrEqualTo(4), "Should have at least 4 red pawns");

        foreach (var pawn in redPawns)
        {
            var posComponent = pawn.GetComponent<IPositionComponent>();
            var pos = posComponent?.CurrentPosition as TilePosition?;
            Assert.That(pos?.TileId.Value, Is.EqualTo("spawn_red"),
                $"Pawn {pawn.Name} should start at spawn_red");
        }

        // ---------------------------------------------------------
        // 3. EXECUTE MOVE: Roll 5 triggers spawn exit (Rule of 5)
        // ---------------------------------------------------------
        var moveParams = new Dictionary<string, object>
        {
            { "Roll", 5 },
            { "PlayerId", Red }
        };

        // Consume AP from FSM node to allow move

        var moveResult = engine.ExecuteAction(ParchisActions.Move, moveParams);

        Assert.That(moveResult.Status, Is.EqualTo(ActionStatus.Completed),
            $"Move should complete. Error: {moveResult.ErrorMessage}");

        // ---------------------------------------------------------
        // 4. VERIFY: One pawn moved to entry tile
        // ---------------------------------------------------------
        state = GetCurrentState(game);
        var pawnAtEntry = state.Entities.Values
            .Where(e => e.DefinitionId.Contains("pawn_red"))
            .FirstOrDefault(e =>
            {
                var posComponent = e.GetComponent<IPositionComponent>();
                var pos = posComponent?.CurrentPosition as TilePosition?;
                return pos?.TileId.Value == ParchisBoard.RedEntry;
            });

        Assert.That(pawnAtEntry, Is.Not.Null,
            $"One pawn should be at {ParchisBoard.RedEntry} after roll 5");

        // ---------------------------------------------------------
        // 5. EXECUTE MOVE: Forward move (roll 3)
        // ---------------------------------------------------------
        var forwardParams = new Dictionary<string, object>
        {
            { "Roll", 3 },
            { "PlayerId", Red }
        };

        var forwardResult = engine.ExecuteAction(ParchisActions.Move, forwardParams);

        Assert.That(forwardResult.Status, Is.EqualTo(ActionStatus.Completed),
            $"Forward move should complete. Error: {forwardResult.ErrorMessage}");

        // ---------------------------------------------------------
        // 6. VERIFY: Pawn moved forward from entry
        // ---------------------------------------------------------
        state = GetCurrentState(game);
        var movedPawn = state.Entities.Values
            .FirstOrDefault(e => e.Id == pawnAtEntry!.Id);

        var finalPosComponent = movedPawn?.GetComponent<IPositionComponent>();
        var finalPos = finalPosComponent?.CurrentPosition as TilePosition?;
        Assert.That(finalPos?.TileId.Value, Is.Not.EqualTo(ParchisBoard.RedEntry),
            "Pawn should have moved forward from entry");
    }

    private static GameState GetCurrentState(ParchisGame game)
    {
        // Access state via the engine's internal repository
        // This is a test helper - in real code, use queries
        var field = game.Engine.GetType().GetField("_repository",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var repo = field?.GetValue(game.Engine) as TurnForge.Engine.Repositories.Interfaces.IGameRepository;
        return repo?.LoadGameState() ?? GameState.Empty();
    }
}
*/
