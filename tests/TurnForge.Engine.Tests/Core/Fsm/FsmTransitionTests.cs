
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Interfaces;
using TurnForge.Engine.Infrastructure.Persistence;
using TurnForge.Engine.Infrastructure.Registration;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Core.Fsm
{
    [TestFixture]
    public class FsmTransitionTests
    {
        // Verified Dummies for Transition
        internal class SpyNode : LeafNode
        {
            public bool OnStartCalled { get; private set; }
            public bool OnEndCalled { get; private set; }

            public override IEnumerable<IFsmApplier> OnStart(global::TurnForge.Engine.Entities.GameState state)
            {
                OnStartCalled = true;
                return base.OnStart(state);
            }

            public override IEnumerable<IFsmApplier> OnEnd(global::TurnForge.Engine.Entities.GameState state)
            {
                OnEndCalled = true;
                return base.OnEnd(state);
            }

            public override bool IsCommandValid(ICommand command, global::TurnForge.Engine.Entities.GameState state) => true;
            public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
            {
                transitionRequested = false;
                return Enumerable.Empty<IFsmApplier>();
            }
        }

        internal class SpyBranch : BranchNode
        {
            public bool OnStartCalled { get; private set; }
            public bool OnEndCalled { get; private set; }

            public override IEnumerable<IFsmApplier> OnStart(global::TurnForge.Engine.Entities.GameState state)
            {
                OnStartCalled = true;
                return base.OnStart(state);
            }

            public override IEnumerable<IFsmApplier> OnEnd(global::TurnForge.Engine.Entities.GameState state)
            {
                OnEndCalled = true;
                return base.OnEnd(state);
            }
        }

        private GameEngineRuntime _runtime;
        private FsmController _controller;
        private InMemoryGameRepository _repository;
        private SpyBranch _root;
        private SpyNode _child1;
        private SpyNode _child2;
        private IEffectSink _effectSink;

        [SetUp]
        public void Setup()
        {
            // 1. Setup Infrastructure
            _effectSink = new ObservableEffectSink();
            var commandBus = new CommandBus(new GameLoop(), new ServiceProviderCommandHandlerResolver(new SimpleServiceProvider()));
            _repository = new InMemoryGameRepository();

            // Initialize Repository with Empty State
            _repository.SaveGameState(global::TurnForge.Engine.Entities.GameState.Empty());

            _runtime = new GameEngineRuntime(commandBus, _effectSink, _repository);

            // 2. Build FSM Tree: Root -> Child1 -> Child2
            // Use Builder to ensure correct linking
            var builder = new GameFlowBuilder();
            var nodeRoot = builder.AddRoot<SpyBranch>("Root", r =>
            {
                r.AddLeaf<SpyNode>("Child1");
                r.AddLeaf<SpyNode>("Child2");
            }).Build();

            // Extract references from built tree to assert on them?
            // "SpyNode" instances are created by the builder via Activator.CreateInstance. 
            // So my `_child1` field above is NOT the one in the tree.
            // I need to traverse the built tree to get the actual instances.
            _root = (SpyBranch)nodeRoot;
            _child1 = (SpyNode)((BranchNode)_root).FirstChild;
            _child2 = (SpyNode)_child1.NextSibling;

            // 3. Init Controller starting at ROOT
            _controller = new FsmController(_root, _root.Id);
            _runtime.SetFsmController(_controller);

            // Force state to have the Initial ID
            var startupState = _repository.LoadGameState().WithCurrentStateId(_root.Id);
            _repository.SaveGameState(startupState);
        }

        [Test]
        public void MoveForward_UpdatesCurrentStateId()
        {
            // Arrange
            var initialState = _repository.LoadGameState();
            Assert.That(initialState.CurrentStateId, Is.EqualTo(_root.Id));

            // Act: Request Move Forward (Root -> Child1)
            var newState = _controller.MoveForwardRequest(initialState, _effectSink);
            _repository.SaveGameState(newState);

            // Assert
            var loadedState = _repository.LoadGameState();
            Assert.That(loadedState.CurrentStateId, Is.EqualTo(_child1.Id), "State should move from Root to Child1");
        }

        [Test]
        public void MoveForward_TriggersOnStart_OnNewNode()
        {
            // Arrange
            var state = _repository.LoadGameState();

            // Act
            var newState = _controller.MoveForwardRequest(state, _effectSink);
            _repository.SaveGameState(newState);

            // Assert
            Assert.That(_child1.OnStartCalled, Is.True, "Child1 OnStart should be called");
            Assert.That(_root.OnEndCalled, Is.True, "Root OnEnd should be called (as we are leaving it to go to child)");
        }

        [Test]
        public void SerialTransitions_Root_Child1_Child2()
        {
            // 1. Root -> Child1
            var state1 = _controller.MoveForwardRequest(_repository.LoadGameState(), _effectSink);
            _repository.SaveGameState(state1);
            Assert.That(_repository.LoadGameState().CurrentStateId, Is.EqualTo(_child1.Id));

            // 2. Child1 -> Child2
            var state2 = _controller.MoveForwardRequest(_repository.LoadGameState(), _effectSink);
            _repository.SaveGameState(state2);

            Assert.That(_repository.LoadGameState().CurrentStateId, Is.EqualTo(_child2.Id));
            Assert.That(_child2.OnStartCalled, Is.True);
            Assert.That(_child1.OnEndCalled, Is.True);
        }
        [Test]
        public void SendCommand_WithStartFSMTag_TriggersMoveForward()
        {
            // Register a dummy command handler that returns StartFSM
            var services = new SimpleServiceProvider();
            services.Register<ICommandHandler<startFsmCommand>>(sp => new startFsmHandler());

            // Re-setup runtime with this registry
            var effectSink = new ObservableEffectSink();
            var resolver = new ServiceProviderCommandHandlerResolver(services);
            var commandBus = new CommandBus(new GameLoop(), resolver);
            _runtime = new GameEngineRuntime(commandBus, effectSink, _repository);
            _runtime.SetFsmController(_controller);

            // Act
            var result = _runtime.Send(new startFsmCommand());
            Assert.That(result.Success, Is.True, $"Command failed: {result.Error}");

            // Assert
            var newState = _repository.LoadGameState();
            Assert.That(newState.CurrentStateId, Is.EqualTo(_child1.Id), "Should move to first child when StartFSM tag is received");
        }

        public record startFsmCommand : ICommand;
        public class startFsmHandler : ICommandHandler<startFsmCommand>
        {
            public CommandResult Handle(startFsmCommand command) => CommandResult.Ok(tags: ["StartFSM"]);
        }
    }
}
