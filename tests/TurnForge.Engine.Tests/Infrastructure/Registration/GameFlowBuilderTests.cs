using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Infrastructure.Registration;

namespace TurnForge.Engine.Tests.Infrastructure.Registration
{
    [TestFixture]
    public class GameFlowBuilderTests
    {
        // ---------------------------------------------------------
        // DUMMY CLASSES MOVED TO GameFlowTestDummies.cs
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        // TESTS
        // ---------------------------------------------------------

        private FsmNode _systemRoot;
        private FsmNode _userRoot;

        [SetUp]
        public void Setup()
        {
            /*
             * Structure:
             * SystemRoot
             *   -> InitialState
             *   -> GamePrepared
             *   -> BattleRound (User Root)
             *       -> MovementPhase ...
             */

            var builder = new GameFlowBuilder();

            _systemRoot = builder
                .AddRoot<BattleRound>("Battle Round", rootConfig =>
                {
                    rootConfig.AddBranch<MovementPhase>("Movement Phase", moveConfig =>
                    {
                        moveConfig.AddLeaf<NormalMove>("Normal Move");
                        moveConfig.AddLeaf<Reinforcements>("Reinforcements");
                    });

                    rootConfig.AddLeaf<ShootingPhase>("Shooting Phase");
                })
                .Build();

            // Extract User Root (3rd child of SystemRoot)
            var systemBranch = _systemRoot as TurnForge.Engine.Core.Fsm.SystemNodes.SystemRootNode;
            if (systemBranch != null && systemBranch.Children.Count >= 3)
            {
                _userRoot = systemBranch.Children.Last();
            }
        }

        [Test]
        public void Validation_SystemStructure()
        {
            Assert.That(_systemRoot, Is.Not.Null);
            Assert.That(_systemRoot, Is.TypeOf<TurnForge.Engine.Core.Fsm.SystemNodes.SystemRootNode>());
            Assert.That(_systemRoot.Parent, Is.Null);

            var branch = (BranchNode)_systemRoot;
            Assert.That(branch.Children.Count, Is.EqualTo(3));

            var init = branch.Children[0];
            var prepared = branch.Children[1];
            var user = branch.Children[2];

            Assert.That(init, Is.TypeOf<TurnForge.Engine.Core.Fsm.SystemNodes.InitialStateNode>());
            Assert.That(prepared, Is.TypeOf<TurnForge.Engine.Core.Fsm.SystemNodes.GamePreparedNode>());
            Assert.That(user, Is.TypeOf<BattleRound>());

            // Check Navigation Linkage
            Assert.That(branch.FirstChild, Is.EqualTo(init));
            Assert.That(init.NextSibling, Is.EqualTo(prepared));
            Assert.That(prepared.NextSibling, Is.EqualTo(user));
            Assert.That(user.NextSibling, Is.Null);
        }

        [Test]
        public void Validation_UserRoot()
        {
            Assert.That(_userRoot, Is.Not.Null);
            Assert.That(_userRoot, Is.TypeOf<BattleRound>());
            Assert.That(_userRoot.Parent, Is.EqualTo(_systemRoot));
            Assert.That(_userRoot.Name, Is.EqualTo("Battle Round"));
        }

        [Test]
        public void Validation_FirstLevelChildren()
        {
            // UserRoot -> MovementPhase
            var rootBranch = _userRoot as BranchNode;
            Assert.That(rootBranch, Is.Not.Null);
            Assert.That(rootBranch!.FirstChild, Is.Not.Null);
            Assert.That(rootBranch.FirstChild, Is.TypeOf<MovementPhase>());

            var movementPhase = rootBranch.FirstChild;
            Assert.That(movementPhase.Name, Is.EqualTo("Movement Phase"));
            Assert.That(movementPhase.Parent, Is.EqualTo(_userRoot));
        }

        [Test]
        public void Validation_Siblings_FirstLevel()
        {
            // MovementPhase -> ShootingPhase
            var rootBranch = _userRoot as BranchNode;
            var movementPhase = rootBranch.FirstChild;

            Assert.That(movementPhase.NextSibling, Is.Not.Null);
            Assert.That(movementPhase.NextSibling, Is.TypeOf<ShootingPhase>());

            var shootingPhase = movementPhase.NextSibling;
            Assert.That(shootingPhase.Name, Is.EqualTo("Shooting Phase"));

            // ShootingPhase -> null
            Assert.That(shootingPhase.NextSibling, Is.Null);
            // ShootingPhase Parent
            Assert.That(shootingPhase.Parent, Is.EqualTo(_userRoot));
        }

        [Test]
        public void Validation_SecondLevelChildren()
        {
            // MovementPhase -> NormalMove
            var rootBranch = _userRoot as BranchNode;
            var movementPhase = rootBranch.FirstChild as BranchNode;

            Assert.That(movementPhase, Is.Not.Null);
            Assert.That(movementPhase.FirstChild, Is.Not.Null);
            Assert.That(movementPhase.FirstChild, Is.TypeOf<NormalMove>());

            var normalMove = movementPhase.FirstChild;
            Assert.That(normalMove.Name, Is.EqualTo("Normal Move"));
            Assert.That(normalMove.Parent, Is.EqualTo(movementPhase));
        }

        [Test]
        public void Validation_Siblings_SecondLevel()
        {
            // NormalMove -> Reinforcements
            var rootBranch = _userRoot as BranchNode;
            var movementPhase = rootBranch!.FirstChild as BranchNode;
            var normalMove = movementPhase.FirstChild;

            Assert.That(normalMove.NextSibling, Is.Not.Null);
            Assert.That(normalMove.NextSibling, Is.TypeOf<Reinforcements>());

            var reinforcements = normalMove.NextSibling;
            Assert.That(reinforcements.Name, Is.EqualTo("Reinforcements"));
            Assert.That(reinforcements.Parent, Is.EqualTo(movementPhase));

            // Reinforcements -> null
            Assert.That(reinforcements.NextSibling, Is.Null);
        }

        [Test]
        public void Validation_Identity()
        {
            var rootBranch = _userRoot as BranchNode;
            var movementPhase = rootBranch.FirstChild;

            // Check IDs
            Assert.That(_systemRoot.Id.Value, Is.Not.EqualTo(System.Guid.Empty));
            Assert.That(_userRoot.Id.Value, Is.Not.EqualTo(System.Guid.Empty));
            Assert.That(movementPhase.Id.Value, Is.Not.EqualTo(System.Guid.Empty));

            Assert.That(_userRoot.Id, Is.Not.EqualTo(movementPhase.Id));
        }
    }
}
