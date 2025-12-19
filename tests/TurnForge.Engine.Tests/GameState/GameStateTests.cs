using System;using NUnit.Framework;using TurnForge.Engine.Entities;using TurnForge.Engine.Entities.Actors;using TurnForge.Engine.ValueObjects;using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Tests.GameState
{
    public class GameStateTests
    {
        [Test]
        public void WithUnit_DoesNotMutateOriginalAndAddsUnit()
        {
            var state = TurnForge.Engine.Entities.GameState.Empty();
            var id = new ActorId(Guid.NewGuid());
            var def = new UnitDefinition(new UnitTypeId("u1"), 10, 3, 2, new System.Collections.Generic.List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
            var unit = new Unit(id, Position.Empty, def);

            var newState = state.WithUnit(unit);

            NUnit.Framework.Assert.That(state.Units.Count, Is.EqualTo(0));
            NUnit.Framework.Assert.That(newState.Units.Count, Is.EqualTo(1));
            NUnit.Framework.Assert.That(newState.Units.ContainsKey(id), Is.True);
            NUnit.Framework.Assert.That(newState, Is.Not.SameAs(state));
        }
    }
}
