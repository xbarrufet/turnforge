using System;
using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.GameState
{
    public class GameStateTests
    {
        [Test]
        public void WithAgent_DoesNotMutateOriginalAndAddsAgent()
        {
            var state = TurnForge.Engine.Entities.GameState.Empty();
            var id = new ActorId(Guid.NewGuid());
            var def = new AgentDefinition(new AgentTypeId("u1"), 10, 3, 2, new System.Collections.Generic.List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
            var agent = new Agent(id, Position.Empty, def);

            var newState = state.WithAgent(agent);

            NUnit.Framework.Assert.That(state.GetAgents().Count, Is.EqualTo(0));
            NUnit.Framework.Assert.That(newState.GetAgents().Count, Is.EqualTo(1));
            // Agents are in a list from GetAgents(), checking existence via ID needs checking the list content or using value 
            // verifying if GetAgents returns list of Agent, check if any agent has that ID.
            NUnit.Framework.Assert.That(newState.GetAgents().Any(a => a.Id == id), Is.True);
            NUnit.Framework.Assert.That(newState, Is.Not.SameAs(state));
        }
    }
}
