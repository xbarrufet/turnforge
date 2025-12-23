using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Infrastructure.Registration;

namespace TurnForge.Engine.Tests.Infrastructure.Registration
{
    [TestFixture]
    public class GameFlowBuilderTests
    {
        private List<FsmNode> _sequence;

        [SetUp]
        public void Setup()
        {
            /*
             * Structure (Flattened):
             * [0] InitialState
             * [1] BoardReady
             * [2] GamePrepared
             * [3] BattleRound (User Root)
             * [4] MovementPhase
             * [5] NormalMove
             * [6] Reinforcements
             * [7] ShootingPhase
             */

            var builder = new GameFlowBuilder();

            _sequence = builder
                .AddNode<BattleRound>("Battle Round") // Used to be Root, now just first node
                .AddNode<MovementPhase>("Movement Phase")
                .AddNode<NormalMove>("Normal Move")
                .AddNode<Reinforcements>("Reinforcements")
                .AddNode<ShootingPhase>("Shooting Phase")
                .Build();
        }

        [Test]
        public void Validation_SequenceLength()
        {
            Assert.That(_sequence, Is.Not.Null);
            Assert.That(_sequence.Count, Is.EqualTo(8)); // 3 System + 5 User
        }

        [Test]
        public void Validation_SystemNodes()
        {
            Assert.That(_sequence[0], Is.TypeOf<TurnForge.Engine.Core.Fsm.SystemNodes.InitialStateNode>());
            Assert.That(_sequence[1], Is.TypeOf<TurnForge.Engine.Core.Fsm.SystemNodes.BoardReadyNode>());
            Assert.That(_sequence[2], Is.TypeOf<TurnForge.Engine.Core.Fsm.SystemNodes.GamePreparedNode>()); // WorldReady
        }

        [Test]
        public void Validation_UserSequenceOrder()
        {
            // [3] BattleRound
            Assert.That(_sequence[3], Is.TypeOf<BattleRound>());
            Assert.That(_sequence[3].Name, Is.EqualTo("Battle Round"));

            // [4] MovementPhase
            Assert.That(_sequence[4], Is.TypeOf<MovementPhase>());
            Assert.That(_sequence[4].Name, Is.EqualTo("Movement Phase"));

            // [5] NormalMove
            Assert.That(_sequence[5], Is.TypeOf<NormalMove>());
            Assert.That(_sequence[5].Name, Is.EqualTo("Normal Move"));

            // [6] Reinforcements
            Assert.That(_sequence[6], Is.TypeOf<Reinforcements>());
            Assert.That(_sequence[6].Name, Is.EqualTo("Reinforcements"));

            // [7] ShootingPhase
            Assert.That(_sequence[7], Is.TypeOf<ShootingPhase>());
            Assert.That(_sequence[7].Name, Is.EqualTo("Shooting Phase"));
        }

        [Test]
        public void Validation_IdsAreUnique()
        {
            var ids = _sequence.Select(n => n.Id.Value).ToList();
            Assert.That(ids.Distinct().Count(), Is.EqualTo(ids.Count), "All node IDs must be unique");
            
            // Check not empty
            foreach(var id in ids)
            {
                Assert.That(id, Is.Not.EqualTo(System.Guid.Empty));
            }
        }
    }
}
