using System;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Definitions;
using TurnForge.Engine.ValueObjects;
namespace TurnForge.Engine.Tests.GameState
{
    public class GameStateTests
    {
        [Test]
        public void WithAgent_DoesNotMutateOriginalAndAddsAgent()
        {
            var state = TurnForge.Engine.Entities.GameState.Empty();
            var id = EntityId.New();
            var def = new AgentDefinition(
                new AgentTypeId("u1"),
                new PositionComponentDefinition(Position.Empty),
                new HealhtComponentDefinition(10),
                new MovementComponentDefinition(3),
                new System.Collections.Generic.List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());

            var agent = new Agent(id, def, new PositionComponent(def.PositionComponentDefinition), new BehaviourComponent(Enumerable.Empty<BaseBehaviour>()));

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
