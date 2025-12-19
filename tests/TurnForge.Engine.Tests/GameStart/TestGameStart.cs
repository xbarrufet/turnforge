using NUnit.Framework;
using TurnForge.Engine.Commands.GameStart;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Tests.Bootstrap;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.GameStart;

[TestFixture]
public class TestGameStart
{
    [Test]
    public void GameStart_with_two_units_verifies_game_state_after_start()
    {
        // Arrange
        var (engine, repository) = EngineTestBootstrapper.Boot();

        // Load a minimal game first
        var spatial = new DiscreteSpatialDescriptor(
            Nodes: new[] { new Position(0, 0), new Position(5, 5) },
            Connections: new[] { new DiscreteConnectionDeacriptor(new Position(0, 0), new Position(5, 5)) }
        );
        var loadCommand = new InitializeGameCommand(spatial: spatial, zones: new List<TurnForge.Engine.Entities.Board.Descriptors.ZoneDescriptor>(), startingProps: Array.Empty<PropDescriptor>());
        engine.Runtime.Send(loadCommand);

        // Verify game is loaded
        var game = repository.GetCurrent();
        Assert.That(game, Is.Not.Null, "Game should be loaded before GameStart");

        // Create two unit descriptors with all required parameters
        var unit1 = new UnitDescriptor(
            TypeId: new UnitTypeId("Survivor1"),
            Position: null, // Strategies will assign position
            ExtraBehaviours: new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()
        );
        var unit2 = new UnitDescriptor(
            TypeId: new UnitTypeId("Survivor2"),
            Position: null,
            ExtraBehaviours: new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()
        );
        
        var playerUnits = new[] { unit1, unit2 };

        // Act: Create and dispatch GameStartCommand through GameEngine
        var gameStartCommand = new GameStartCommand(playerUnits);
        engine.Runtime.Send(gameStartCommand);

        // Assert: Game should still exist in repository
        var gameAfterStart = repository.GetCurrent();
        Assert.That(gameAfterStart, Is.Not.Null, "Game should still be in repository after creating GameStartCommand");
        Assert.That(gameAfterStart.GameBoard, Is.Not.Null, "Game should have a board");
    }
}
