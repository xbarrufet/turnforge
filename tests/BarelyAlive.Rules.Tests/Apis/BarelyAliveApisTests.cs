using BarelyAlive.Rules.Game;
using NUnit.Framework;
using TurnForge.Engine.Commands.GameStart.Effects;

namespace BarelyAlive.Rules.Tests.Apis;

[TestFixture]
public class BarelyAliveApisTests
{
    private string _json;

    [SetUp]
    public void Setup()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Assets", "all_mission01.json");
        _json = File.ReadAllText(path);
    }

    [Test]
    public void InitializeGame_ShouldSucceed_AndEmitEffects()
    {
        // Arrange
        var game = BarelyAliveGame.CreateNewGame();

        // Act
        var result = game.BarelyAliveApis.InitializeGame(_json);

        // Assert
        Assert.That(result.Success, Is.True, $"API call failed: {result.Error}");
        Assert.That(result.Error, Is.Null, "API call should not return errors");

        // Verify side effects
        Assert.That(game.EventHandler.LastEffect, Is.Not.Null, "An effect should have been emitted");
        Assert.That(game.EventHandler.LastEffect, Is.InstanceOf<PropSpawnedEffect>(), "Effect should be PropSpawnedEffect");
    }
}
