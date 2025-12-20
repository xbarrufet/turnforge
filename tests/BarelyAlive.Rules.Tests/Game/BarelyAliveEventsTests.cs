using BarelyAlive.Rules.Events;
using BarelyAlive.Rules.Events.Interfaces;
using BarelyAlive.Rules.Game;
using NUnit.Framework;
using TurnForge.Engine.Commands.GameStart.Effects;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Events;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Game;

[TestFixture]
public class BarelyAliveEventsTests
{
    [Test]
    public void Subscribe_ShouldReceiveTransformedEffects()
    {
        // Arrange
        var game = BarelyAliveGame.CreateNewGame();
        IBarelyAliveEffect? receivedEffect = null;

        game.Subscribe(effect => receivedEffect = effect);

        var entityId = EntityId.New();
        var agentTypeId = new AgentTypeId("SurvivorType");
        var position = new Position(new Vector(1, 2));
        var gameEffect = new AgentSpawnedEffect(entityId, agentTypeId, position);

        // Act
        game.EventHandler.Handle(gameEffect);

        // Assert
        Assert.That(receivedEffect, Is.Not.Null, "Should have received an effect");
        Assert.That(receivedEffect, Is.InstanceOf<AgentSpawned>(), "Should be an AgentSpawned effect");

        var agentSpawned = (AgentSpawned)receivedEffect!;
        Assert.That(agentSpawned.AgentId, Is.EqualTo(entityId.ToString()));
        Assert.That(agentSpawned.AgentType, Is.EqualTo(agentTypeId.ToString()));
        Assert.That(agentSpawned.AreaId, Is.EqualTo(position.ToString()));
    }

    [Test]
    public void Handle_ShouldNotThrowOnUnknownEffect()
    {
        // Arrange
        var game = BarelyAliveGame.CreateNewGame();
        var unknownEffect = new TestUnknownEffect();

        // Act & Assert
        Assert.DoesNotThrow(() => game.EventHandler.Handle(unknownEffect));
    }

    private class TestUnknownEffect : TurnForge.Engine.Core.Interfaces.IGameEffect { }
}
