using BarelyAlive.Rules.Game;
using NUnit.Framework;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace BarelyAlive.Rules.Tests.Game;

[TestFixture]
public class BarelyAliveGameStartupTests
{
    [Test]
    public void CreateNewGame_ShouldInitializeSuccessfully()
    {
        // Act
        var game = BarelyAliveGame.CreateNewGame();

        // Assert
        Assert.That(game, Is.Not.Null, "Game instance should be created");
    }

    [Test]
    public void CreateNewGame_ShouldRegisterSurvivors()
    {
        // Arrange
        var game = BarelyAliveGame.CreateNewGame();

        // Act - Try to retrieve registered survivors
        var amyDefinition = game.GameCatalog.GetAgentDefinition("Amy");
        var dougDefinition = game.GameCatalog.GetAgentDefinition("Doug");

        // Assert
        Assert.That(amyDefinition, Is.Not.Null, "Amy should be registered");
        Assert.That(amyDefinition.AgentName, Is.EqualTo("Amy"));
        Assert.That(amyDefinition.HealthComponentDefinition.MaxHealth, Is.EqualTo(3), "Amy should have 3 health");
        // Assert.That(amyDefinition.MaxActionPoints, Is.EqualTo(3), "Amy should have 3 action points"); // TODO: ActionPoints definitions are pending refactor
        Assert.That(amyDefinition.MovementComponentDefinition.MaxUnitsToMove, Is.EqualTo(1), "Amy should have 1 movement");

        Assert.That(dougDefinition, Is.Not.Null, "Doug should be registered");
        Assert.That(dougDefinition.AgentName, Is.EqualTo("Doug"));
        Assert.That(dougDefinition.HealthComponentDefinition.MaxHealth, Is.EqualTo(3), "Doug should have 3 health");
        // Assert.That(dougDefinition.MaxActionPoints, Is.EqualTo(3), "Doug should have 3 action points"); // TODO: ActionPoints definitions are pending refactor
        Assert.That(dougDefinition.MovementComponentDefinition.MaxUnitsToMove, Is.EqualTo(1), "Doug should have 1 movement");
    }

    [Test]
    public void CreateNewGame_ShouldRegisterZombies()
    {
        // Arrange
        var game = BarelyAliveGame.CreateNewGame();

        // Act - Try to retrieve registered zombies
        var walkerDefinition = game.GameCatalog.GetAgentDefinition("Walker");
        var runnerDefinition = game.GameCatalog.GetAgentDefinition("Runner");
        var fattyDefinition = game.GameCatalog.GetAgentDefinition("Fatty");

        // Assert
        Assert.That(walkerDefinition, Is.Not.Null, "Walker should be registered");
        Assert.That(walkerDefinition.AgentName, Is.EqualTo("Walker"));
        Assert.That(walkerDefinition.HealthComponentDefinition.MaxHealth, Is.EqualTo(1), "Walker should have 1 health");
        // Assert.That(walkerDefinition.MaxActionPoints, Is.EqualTo(1), "Walker should have 1 action point"); // TODO: ActionPoints definitions are pending refactor

        Assert.That(runnerDefinition, Is.Not.Null, "Runner should be registered");
        Assert.That(runnerDefinition.AgentName, Is.EqualTo("Runner"));
        Assert.That(runnerDefinition.HealthComponentDefinition.MaxHealth, Is.EqualTo(1), "Runner should have 1 health");
        // Assert.That(runnerDefinition.MaxActionPoints, Is.EqualTo(2), "Runner should have 2 action points"); // TODO: ActionPoints definitions are pending refactor

        Assert.That(fattyDefinition, Is.Not.Null, "Fatty should be registered");
        Assert.That(fattyDefinition.AgentName, Is.EqualTo("Fatty"));
        Assert.That(fattyDefinition.HealthComponentDefinition.MaxHealth, Is.EqualTo(2), "Fatty should have 2 health");
        // Assert.That(fattyDefinition.MaxActionPoints, Is.EqualTo(1), "Fatty should have 1 action point"); // TODO: ActionPoints definitions are pending refactor
    }

    [Test]
    public void CreateNewGame_ShouldRegisterProps()
    {
        // Arrange
        var game = BarelyAliveGame.CreateNewGame();

        // Act - Try to retrieve registered prop
        var spawnDefinition = game.GameCatalog.GetPropDefinition(new PropTypeId("BarelyAlive.Spawn"));

        // Assert
        Assert.That(spawnDefinition, Is.Not.Null, "BarelyAlive.Spawn should be registered");
        Assert.That(spawnDefinition.TypeId.Value, Is.EqualTo("BarelyAlive.Spawn"));
        Assert.That(spawnDefinition.HealhtComponentDefinition.MaxHealth, Is.EqualTo(1), "Spawn should have 1 health");
        // Assert.That(spawnDefinition.MaxBaseMovement, Is.EqualTo(0), "Spawn should have 0 movement");
        // Assert.That(spawnDefinition.MaxActionPoints, Is.EqualTo(0), "Spawn should have 0 action points");
    }
}
