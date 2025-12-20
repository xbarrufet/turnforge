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

        private FsmNode _builtRoot;

        [SetUp]
        public void Setup()
        {
            /*
             * Structure:
             * Root: BattleRound (Branch)
             *    Child 1: MovementPhase (Branch)
             *       Sub-Child 1.1: NormalMove (Leaf)
             *       Sub-Child 1.2: Reinforcements (Leaf)
             *    Child 2: ShootingPhase (Leaf)
             */

            var builder = new GameFlowBuilder();

            _builtRoot = builder
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
        }

        [Test]
        public void Validation_Root()
        {
            Assert.That(_builtRoot, Is.Not.Null);
            Assert.That(_builtRoot, Is.TypeOf<BattleRound>());
            Assert.That(_builtRoot.Parent, Is.Null, "Root should not have a parent.");
            Assert.That(_builtRoot.NextSibling, Is.Null, "Root should not have a sibling.");
            Assert.That(_builtRoot.Name, Is.EqualTo("Battle Round"));
        }

        [Test]
        public void Validation_FirstLevelChildren()
        {
            // Root -> MovementPhase
            var rootBranch = _builtRoot as BranchNode;
            Assert.That(rootBranch, Is.Not.Null);
            Assert.That(rootBranch!.FirstChild, Is.Not.Null);
            Assert.That(rootBranch.FirstChild, Is.TypeOf<MovementPhase>());

            var movementPhase = rootBranch.FirstChild;
            Assert.That(movementPhase.Name, Is.EqualTo("Movement Phase"));
            Assert.That(movementPhase.Parent, Is.EqualTo(_builtRoot));
        }

        [Test]
        public void Validation_Siblings_FirstLevel()
        {
            // MovementPhase -> ShootingPhase
            var rootBranch = _builtRoot as BranchNode;
            var movementPhase = rootBranch.FirstChild;

            Assert.That(movementPhase.NextSibling, Is.Not.Null);
            Assert.That(movementPhase.NextSibling, Is.TypeOf<ShootingPhase>());

            var shootingPhase = movementPhase.NextSibling;
            Assert.That(shootingPhase.Name, Is.EqualTo("Shooting Phase"));

            // ShootingPhase -> null
            Assert.That(shootingPhase.NextSibling, Is.Null);
            // ShootingPhase Parent
            Assert.That(shootingPhase.Parent, Is.EqualTo(_builtRoot));
        }

        [Test]
        public void Validation_SecondLevelChildren()
        {
            // MovementPhase -> NormalMove
            var rootBranch = _builtRoot as BranchNode;
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
            var rootBranch = _builtRoot as BranchNode;
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
            var rootBranch = _builtRoot as BranchNode;
            var movementPhase = rootBranch.FirstChild;

            // Check IDs are not empty
            Assert.That(_builtRoot.Id.Value, Is.Not.EqualTo(System.Guid.Empty));
            Assert.That(movementPhase.Id.Value, Is.Not.EqualTo(System.Guid.Empty));

            // Check IDs are different
            Assert.That(_builtRoot.Id, Is.Not.EqualTo(movementPhase.Id));
        }
    }
}
