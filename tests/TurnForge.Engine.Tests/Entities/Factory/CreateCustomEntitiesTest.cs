using NUnit.Framework;
using TurnForge.Engine.Core.Exceptions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Services;
using TurnForge.Engine.Tests.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Entities.Factory
{
    [TestFixture]
    public class CreateCustomEntitiesTest
    {
        private InMemoryGameCatalog _registry = null!;
        private GenericEntityFactory _factory = null!;

        private string _customSpeedTraitAutoInitializeDefinitionId = "custom-speed-trait-auto-init-def";
        private string _customSpeedTraitRequireInitializeDefinitionId = "custom-speed-trait-requires-init-def";
        
        [SetUp]
        public void SetUp()
        {
            _registry = new InMemoryGameCatalog();
            _registry.RegisterDefinition<CustomSpeedInitializedAutomaticallyDefinition>(new CustomSpeedInitializedAutomaticallyDefinition(_customSpeedTraitAutoInitializeDefinitionId));
            _registry.RegisterDefinition<CustomSpeedAgentRequiresInitializationDefinition>(new CustomSpeedAgentRequiresInitializationDefinition(_customSpeedTraitRequireInitializeDefinitionId));
            _factory = new GenericEntityFactory(_registry);
        }

        [Test]
        public void Create_Custom_Agent_With_that_fails_due_to_no_initailization()
        {
            // Arrange: register custom definition with a custom trait
            var incompleteAgentDescriptor = new CustomTestAgentDescriptor(
                _customSpeedTraitRequireInitializeDefinitionId, teamId: "blue", playerId: new PlayerId("player_1"),
                new LimboPositionId());
            Assert.Throws<InvalidDescriptorException>(() =>
            {
                Agent agent = _factory.BuildAgent(incompleteAgentDescriptor);
            });

        }
        [Test]
        public void Create_Custom_Agent_with_automatic_initialization()
        {
             // Act: build agent via factory
             var completeAgentDescriptor = new CustomTestAgentDescriptor(
                 _customSpeedTraitAutoInitializeDefinitionId, teamId: "blue", playerId: new PlayerId("player_1"),
                 new LimboPositionId());
             Agent agent = _factory.BuildAgent(completeAgentDescriptor);
            // Assert: agent has trait and component materialized
            Assert.That(agent, Is.Not.Null);
            Assert.That(agent.HasTrait<CustomSpeedTrait>(), Is.True);
            Assert.That(agent.TryGetComponent<CustomSpeedComponent>(out var comp), Is.True);
            Assert.That(comp!.MaxSpeed, Is.EqualTo(9));
            Assert.That(comp.CurrentSpeed, Is.EqualTo(0));
        }
      
        
        [Test]
        public void Create_Custom_Agent_With_required_Custom_Trait_And_Component_overriding_trait_value()
        {
            
            var descriptor = new CustomTestAgentDescriptor(_customSpeedTraitRequireInitializeDefinitionId, teamId: "blue", playerId: new PlayerId("player_1")
                ,new LimboPositionId(),new CustomSpeedTrait(10));

            // Act: build agent via factory
            Agent agent = _factory.BuildAgent(descriptor);

            // Assert: agent has trait and component materialized
            Assert.That(agent, Is.Not.Null);
            Assert.That(agent.HasTrait<CustomSpeedTrait>(), Is.True);
            Assert.That(agent.TryGetComponent<CustomSpeedComponent>(out var comp), Is.True);
            Assert.That(comp!.MaxSpeed, Is.EqualTo(10));
            Assert.That(comp.CurrentSpeed, Is.EqualTo(0));
        }
        
        
        [Test]
        public void Create_Custom_Agent_With_autaoinitialized_Custom_Trait_And_Component_overriding_trait_value_and_component()
        {
            var descriptor = new CustomTestAgentDescriptor(_customSpeedTraitRequireInitializeDefinitionId, teamId: "blue", playerId: new PlayerId("player_1"),startPosition:new LimboPositionId(),new CustomSpeedComponent(new CustomSpeedTrait(10),5),new CustomSpeedTrait(10));

            // Act: build agent via factory
            Agent agent = _factory.BuildAgent(descriptor);

            // Assert: agent has trait and component materialized
            Assert.That(agent, Is.Not.Null);
            Assert.That(agent.HasTrait<CustomSpeedTrait>(), Is.True);
            Assert.That(agent.TryGetComponent<CustomSpeedComponent>(out var comp), Is.True);
            Assert.That(comp!.MaxSpeed, Is.EqualTo(10));
            Assert.That(comp.CurrentSpeed, Is.EqualTo(5));
        }
    }
}

