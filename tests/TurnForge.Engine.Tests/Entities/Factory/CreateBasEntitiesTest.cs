using System;
using Moq;
using NUnit.Framework;
using TurnForge.Engine.Core.Exceptions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Definitions.BasicDefinitions;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.Entities.Definitions.CoreBase;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Infrastructure.Registration;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Services;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Entities.Factory
{
    // Example definition type for tests
    public sealed record TestDef(string Name);

    [TestFixture]
    public class CreateBasEntitiesTest
    {
        // OPTION B: Use the real in-memory registry (simple and reliable)
        private InMemoryGameCatalog _realRegistry = null!;
        private readonly string _testBasicAgentDefinitionId = "test-agent-def";
        private readonly Category _testAgenCategory = new Category("TestAgentCategory");
        private readonly string _testBasicPropDefinitionId = "test-prop-def";
        private readonly Category _testPropCategory = new Category("TestPropCategory");
        private readonly string _testBasicConnectionDefinitionId = "test-conn-def";
        private readonly Category _testonnectionCategory = new Category("TestConnectionCategory");
        private readonly string _testBasicZoneDefinitionId = "test-zone-def";
        private readonly Category _testZoneCategoy = new("TestZoneCategory");

        private GenericEntityFactory _entityFactory;


        [SetUp]
        public void SetUp()
        {
            _realRegistry = new InMemoryGameCatalog();
            _realRegistry.RegisterDefinition<AgentDefinition>(_testBasicAgentDefinitionId, _testAgenCategory);
            _realRegistry.RegisterDefinition<PropDefinition>(_testBasicPropDefinitionId, _testPropCategory);
            _realRegistry.RegisterDefinition<ConnectionDefinition>(_testBasicConnectionDefinitionId,
                _testonnectionCategory);
            _realRegistry.RegisterDefinition<ZoneDefinition>(_testBasicZoneDefinitionId, _testZoneCategoy);
            _entityFactory = new GenericEntityFactory(_realRegistry);

        }

        [Test]
        public void Incomplete_Agent_Descriptor_Throws_Exception()
        {
            PlayerId playerId = new PlayerId("Player1");
            TeamId teamId = new TeamId("Team1");

            var incompleteAgentDescriptor = new TestIncompleteDescriptor(
                teamId: teamId,
                playerId: playerId
            );

            Assert.Throws<InvalidDescriptorException>(() =>
            {
                Agent agent = _entityFactory.BuildAgent(incompleteAgentDescriptor);
            });
        }

        [Test]
        public void Create_Vanilla_Agent_no_definition_no_position()
        {
            PlayerId playerId = new PlayerId("Player1");
            TeamId teamId = new TeamId("Team1");

            var agentDescriptorWithDefinition = new TestAgentDescriptor(
                teamId: teamId,
                playerId: playerId
            );

            Agent agent = _entityFactory.BuildAgent(agentDescriptorWithDefinition);
            Assert.That(agent, Is.Not.Null);
            //asert category matches definition
            Assert.That(agent.Category, Is.EqualTo(Agent.AgentDefaultCategory));
            // Assert direct properties (Team, Controller)
            Assert.That(agent.Controller, Is.EqualTo(playerId));
            Assert.That(agent.Team, Is.EqualTo(teamId));
            // Assert default position (direct property)
            Assert.That(agent.CurrentPosition, Is.EqualTo(new LimboPositionId()));
            //assert definition
            Assert.That(agent.DefinitionId, Is.EqualTo(BasicAgentDefinition.DefinitionId));

        }

        [Test]
        public void Create_Vanilla_Prop_no_definition_no_position()
        {
            var propDescriptor = new TestPropDescriptor();

            Prop prop = _entityFactory.BuildProp(propDescriptor);
            Assert.That(prop, Is.Not.Null);
            // assert category matches definition
            Assert.That(prop.Category, Is.EqualTo(Prop.PropDefaultCategory));
            // Assert default position (direct property)
            Assert.That(prop.CurrentPosition, Is.EqualTo(new LimboPositionId()));
            // assert definition
            Assert.That(prop.DefinitionId, Is.EqualTo(BasicPropDefinition.DefinitionId));
        }

        [Test]
        public void Create_Vanilla_Agent_no_definition_with_position()
        {
            PlayerId playerId = new PlayerId("Player1");
            TeamId teamId = new TeamId("Team1");

            var tileId = new TileId("tile-1");
            var agentDescriptorWithDefinition = new TestAgentDescriptor(
                teamId: teamId,
                startPosition: tileId,
                playerId: playerId
            );

            Agent agent = _entityFactory.BuildAgent(agentDescriptorWithDefinition);
            Assert.That(agent, Is.Not.Null);
            // Assert position (direct property)
            Assert.That(agent.CurrentPosition, Is.EqualTo(tileId));

        }

        [Test]
        public void Create_Vanilla_Prop_no_definition_with_position()
        {
            PlayerId playerId = new PlayerId("Player1");
            TeamId teamId = new TeamId("Team1");

            var tileId = new TileId("tile-1");
            var propWithPosition = new TestPropDescriptor(
                startPosition: tileId
            );

            var prop = _entityFactory.BuildProp(propWithPosition);
            Assert.That(prop, Is.Not.Null);
            // Assert position (direct property)
            Assert.That(prop.CurrentPosition, Is.EqualTo(tileId));

        }

        [Test]
        public void Create_Agent_with_definition_and_position()
        {
            PlayerId playerId = new PlayerId("Player1");
            TeamId teamId = new TeamId("Team1");

            var tileId = new TileId("tile-1");
            var agentDescriptorWithDefinition = new TestAgentDescriptor(
                teamId: teamId,
                definitionId: _testBasicAgentDefinitionId,
                startPosition: tileId,
                playerId: playerId
            );

            Agent agent = _entityFactory.BuildAgent(agentDescriptorWithDefinition);
            Assert.That(agent, Is.Not.Null);
            // assert definition
            Assert.That(agent.DefinitionId, Is.EqualTo(_testBasicAgentDefinitionId));

        }
    }
}
