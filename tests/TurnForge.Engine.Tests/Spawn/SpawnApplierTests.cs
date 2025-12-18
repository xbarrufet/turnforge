using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Infrastructure.Appliers;
using TurnForge.Engine.Infrastructure.Appliers.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Tests.Spawn
{
    public class SpawnApplierTests
    {
        private class TestEffectSink : TurnForge.Engine.Core.Interfaces.IEffectSink
        {
            public readonly List<TurnForge.Engine.Core.Interfaces.IGameEffect> Emitted = new();
            public void Emit(TurnForge.Engine.Core.Interfaces.IGameEffect effect) => Emitted.Add(effect);
        }

        private class TestActorFactory : TurnForge.Engine.Entities.Actors.Interfaces.IActorFactory
        {
            public Unit? LastBuiltUnit;
            public Prop? LastBuiltProp;

            public Unit BuildUnit(TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                var id = new ActorId(System.Guid.NewGuid());
                var def = new UnitDefinition(typeId, 10, behaviours??new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
                var u = new Unit(id, position, def, behaviours?.ToList());
                LastBuiltUnit = u;
                return u;
            }

            public Prop BuildProp(TurnForge.Engine.Entities.Actors.Definitions.PropTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                var id = new ActorId(System.Guid.NewGuid());
                var def = new PropDefinition(typeId, null, behaviours??new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
                var p = new Prop(id, position, def, null, behaviours?.ToList());
                LastBuiltProp = p;
                return p;
            }

            public Npc BuildNpc(TurnForge.Engine.Entities.Actors.Definitions.NpcTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Apply_AddsUnitsAndEmitsEffects()
        {
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();
            var applier = new SpawnApplier(factory, effectSink);

            var state = TurnForge.Engine.Entities.GameState.Empty();
            var decisions = new List<TurnForge.Engine.Strategies.Spawn.Interfaces.ISpawnDecision>
            {
                new UnitSpawnDecision(new TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId("uType"), Position.Zero, new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()),
                new PropSpawnDecision(new TurnForge.Engine.Entities.Actors.Definitions.PropTypeId("pType"), Position.Zero, new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>())
            };

            var newState = applier.Apply(decisions, state);

            NUnit.Framework.Assert.That(newState.Units.Count, Is.EqualTo(1));
            NUnit.Framework.Assert.That(newState.Props.Count, Is.EqualTo(1));
            NUnit.Framework.Assert.That(effectSink.Emitted.Count, Is.EqualTo(2));
            NUnit.Framework.Assert.That(factory.LastBuiltUnit, Is.Not.Null);
            NUnit.Framework.Assert.That(factory.LastBuiltProp, Is.Not.Null);
        }
    }
}
