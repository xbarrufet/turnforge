
using System.Collections.Generic;
using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Strategies.Spawn
{
    [TestFixture]
    public class AgentNpcSpawnStrategyTests
    {
        private class StubNpcStrategy : IAgentNpcSpawnStrategy
        {
            public IReadOnlyList<AgentSpawnDecision> Decide(AgentNpcSpawnContext context)
            {
                // Simple logic: spawn 1 NPC at (0,0) if GameState is empty, otherwise 0
                if (context.GameState.GetAgents().Count == 0)
                {
                    return new List<AgentSpawnDecision>
// ...
                    {
                        new AgentSpawnDecision(new AgentTypeId("Npc1"), new Position(Vector.Zero), new List<IActorBehaviour>())
                    };
                }
                return new List<AgentSpawnDecision>();
            }
        }

        [Test]
        public void Decide_WithEmptyState_ReturnsDecision()
        {
            // Arrange
            var strategy = new StubNpcStrategy();
            var state = TurnForge.Engine.Entities.GameState.Empty();
            var context = new AgentNpcSpawnContext(state);

            // Act
            var decisions = strategy.Decide(context);

            // Assert
            Assert.That(decisions.Count, Is.EqualTo(1));
            Assert.That(decisions[0].TypeId, Is.EqualTo(new AgentTypeId("Npc1")));
            Assert.That(decisions[0].Position, Is.EqualTo(new Position(Vector.Zero)));
        }

        [Test]
        public void Context_HoldsGameState()
        {
            // Arrange
            var state = TurnForge.Engine.Entities.GameState.Empty();
            var context = new AgentNpcSpawnContext(state);

            // Assert
            Assert.That(context.GameState, Is.SameAs(state));
        }
    }
}
