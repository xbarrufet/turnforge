using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Definitions;
using TurnForge.Engine.Entities.Decisions.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.ValueObjects;
namespace TurnForge.Engine.Tests.Spawn
{
    public class SpawnApplierTests
    {
        private class TestEffectSink : TurnForge.Engine.Core.Interfaces.IEffectSink
        {
            public readonly List<TurnForge.Engine.Core.Interfaces.IGameEffect> Emitted = new();
            public void Emit(TurnForge.Engine.Core.Interfaces.IGameEffect effect) => Emitted.Add(effect);
            public void Subscribe(System.Action<TurnForge.Engine.Core.Interfaces.IGameEffect> action) { } // No-op for test
        }

        private class TestActorFactory : TurnForge.Engine.Entities.Actors.Interfaces.IActorFactory
        {
            public Agent? LastBuiltAgent;
            public Prop? LastBuiltProp;

            public Agent BuildAgent(AgentDescriptor descriptor) => Build(descriptor);
            public Prop BuildProp(PropDescriptor descriptor) => Build(descriptor);

            public Agent Build(IGameEntityDescriptor<Agent> descriptor)
            {
                var d = (AgentDescriptor)descriptor;
                var id = EntityId.New();
                var behavioursList = d.ExtraBehaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
                var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(d.ExtraBehaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

                var def = new AgentDefinition(d.TypeId,
                    new PositionComponentDefinition(Position.Empty),
                    new HealhtComponentDefinition(10),
                    new MovementComponentDefinition(3),
                    behavioursList);

                var u = new Agent(id, def, new PositionComponent(def.PositionComponentDefinition), component);
                LastBuiltAgent = u;
                return u;
            }

            public Prop Build(IGameEntityDescriptor<Prop> descriptor)
            {
                var d = (PropDescriptor)descriptor;
                var id = EntityId.New();
                var behavioursList = d.ExtraBehaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
                var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(d.ExtraBehaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

                var def = new PropDefinition(d.TypeId,
                    new PositionComponentDefinition(Position.Empty),
                    new HealhtComponentDefinition(10),
                    behavioursList);

                var p = new Prop(id, def, new PositionComponent(def.PositionComponentDefinition), component);
                LastBuiltProp = p;
                return p;
            }


        }

        [Test]
        public void Apply_AddsAgentsAndEmitsEffects()
        {
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();
            var applier = new SpawnApplier(factory, effectSink);

            var state = TurnForge.Engine.Entities.GameState.Empty();
            var decisions = new List<ISpawnDecision>
            {
                new AgentSpawnDecision(new TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId("uType"), new Position(Vector.Zero), new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>()),
                new PropSpawnDecision(new TurnForge.Engine.Entities.Actors.Definitions.PropTypeId("pType"), new Position(Vector.Zero), new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>())
            };

            var newState = applier.Apply(decisions, state);

            NUnit.Framework.Assert.That(newState.GetAgents().Count, Is.EqualTo(1));
            NUnit.Framework.Assert.That(newState.GetProps().Count, Is.EqualTo(1));
            NUnit.Framework.Assert.That(effectSink.Emitted.Count, Is.EqualTo(2));
            NUnit.Framework.Assert.That(factory.LastBuiltAgent, Is.Not.Null);
            NUnit.Framework.Assert.That(factory.LastBuiltProp, Is.Not.Null);
        }


        [Test]
        public void Apply_PropWithBehaviour_BehaviourIsAddedToEntity()
        {
            // Arrange
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();
            var applier = new SpawnApplier(factory, effectSink);

            var state = TurnForge.Engine.Entities.GameState.Empty();

            // Create a test behaviour
            var testBehaviour = new TestBehaviour();
            var behaviours = new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour> { testBehaviour };

            var decisions = new List<ISpawnDecision>
            {
                new PropSpawnDecision(
                    new TurnForge.Engine.Entities.Actors.Definitions.PropTypeId("PropWithBehaviour"),
                    new Position(Vector.Zero),
                    behaviours)
            };

            // Act
            var newState = applier.Apply(decisions, state);

            // Assert
            NUnit.Framework.Assert.That(newState.Props.Count, Is.EqualTo(1), "Should have spawned 1 prop");
            NUnit.Framework.Assert.That(factory.LastBuiltProp, Is.Not.Null, "Factory should have built a prop");
            var behaviourComponent = factory.LastBuiltProp!.GetComponent<BehaviourComponent>();
            NUnit.Framework.Assert.That(behaviourComponent, Is.Not.Null, "Prop should have BehaviourComponent");
            NUnit.Framework.Assert.That(behaviourComponent!.Behaviours, Is.Not.Null, "Prop should have behaviours collection");
            NUnit.Framework.Assert.That(behaviourComponent.Behaviours.Count, Is.EqualTo(1), "Prop should have 1 behaviour");
            NUnit.Framework.Assert.That(behaviourComponent.Behaviours[0], Is.SameAs(testBehaviour), "Prop should have the exact behaviour instance");
        }

        // Test behaviour implementation
        private class TestBehaviour : TurnForge.Engine.Entities.Actors.ActorBehaviour
        {
        }

        [Test]
        public void Apply_PropWithParameterizedBehaviour_BehaviourWithCorrectParametersIsAddedToEntity()
        {
            // Arrange
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();
            var applier = new SpawnApplier(factory, effectSink);

            var state = TurnForge.Engine.Entities.GameState.Empty();

            // Create a parameterized behaviour
            var expectedOrder = 5;
            var expectedMaxSpawns = 10;
            var parameterizedBehaviour = new TestParameterizedBehaviour(expectedOrder, expectedMaxSpawns);
            var behaviours = new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour> { parameterizedBehaviour };

            var decisions = new List<ISpawnDecision>
            {
                new PropSpawnDecision(
                    new TurnForge.Engine.Entities.Actors.Definitions.PropTypeId("PropWithParameterizedBehaviour"),
                    new Position(Vector.Zero),
                    behaviours)
            };

            // Act
            var newState = applier.Apply(decisions, state);

            // Assert
            NUnit.Framework.Assert.That(newState.Props.Count, Is.EqualTo(1), "Should have spawned 1 prop");
            NUnit.Framework.Assert.That(factory.LastBuiltProp, Is.Not.Null, "Factory should have built a prop");
            var behaviourComponent = factory.LastBuiltProp!.GetComponent<BehaviourComponent>();
            NUnit.Framework.Assert.That(behaviourComponent, Is.Not.Null, "Prop should have BehaviourComponent");
            NUnit.Framework.Assert.That(behaviourComponent!.Behaviours, Is.Not.Null, "Prop should have behaviours collection");
            NUnit.Framework.Assert.That(behaviourComponent.Behaviours.Count, Is.EqualTo(1), "Prop should have 1 behaviour");

            var spawnedBehaviour = behaviourComponent.Behaviours[0] as TestParameterizedBehaviour;
            NUnit.Framework.Assert.That(spawnedBehaviour, Is.Not.Null, "Behaviour should be of type TestParameterizedBehaviour");
            NUnit.Framework.Assert.That(spawnedBehaviour!.Order, Is.EqualTo(expectedOrder), "Behaviour should have correct Order parameter");
            NUnit.Framework.Assert.That(spawnedBehaviour.MaxSpawns, Is.EqualTo(expectedMaxSpawns), "Behaviour should have correct MaxSpawns parameter");
        }

        // Test parameterized behaviour implementation
        private class TestParameterizedBehaviour : TurnForge.Engine.Entities.Actors.ActorBehaviour
        {
            public int Order { get; }
            public int MaxSpawns { get; }

            public TestParameterizedBehaviour(int order, int maxSpawns)
            {
                Order = order;
                MaxSpawns = maxSpawns;
            }
        }
    }
}
