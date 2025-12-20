using BarelyAlive.Rules.Game;
using NUnit.Framework;
using TurnForge.Engine.Strategies.Spawn;

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
    public void InitializeGame_ShouldSucceed_AndReturnDecisions()
    {
        // Arrange
        var game = BarelyAliveGame.CreateNewGame();

        // Act
        var result = game.BarelyAliveApis.InitializeGame(_json);

        // Assert
        Assert.That(result.Success, Is.True, $"API call failed: {result.Error}");
        Assert.That(result.Error, Is.Null, "API call should not return errors");

        // Verify decisions (Orchestrator not present, so we verify intent)
        Assert.That(result.Decisions, Is.Not.Null, "Decisions collection should not be null");
        Assert.That(result.Decisions, Is.Not.Empty, "Should have generated decisions");

        var propDecisions = result.Decisions.OfType<PropSpawnDecision>().ToList();
        Assert.That(propDecisions, Is.Not.Empty, "Should have generated PropSpawnDecisions");
    }
}
