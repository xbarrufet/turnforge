
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
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Infrastructure.Interfaces;
using TurnForge.Engine.Infrastructure.Persistence;
using TurnForge.Engine.Infrastructure.Registration;
using TurnForge.Engine.Core.Orchestrator; // Added
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Decisions.Entity.Interfaces;

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
            public bool ShouldTransitionOnCommand { get; set; } = false;
            public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
            {
                transitionRequested = ShouldTransitionOnCommand;
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

        internal class StubBoardFactory : TurnForge.Engine.Entities.Board.Interfaces.IBoardFactory
        {
            public TurnForge.Engine.Entities.Board.GameBoard Build(TurnForge.Engine.Entities.Descriptors.Interfaces.IGameEntityDescriptor<TurnForge.Engine.Entities.Board.GameBoard> descriptor) => new TurnForge.Engine.Entities.Board.GameBoard(new TurnForge.Engine.Spatial.ConnectedGraphSpatialModel(new TurnForge.Engine.Spatial.MutableTileGraph(new HashSet<TurnForge.Engine.ValueObjects.TileId>())));
        }

        private GameEngineRuntime _runtime;
        private FsmController _controller;
        private InMemoryGameRepository _repository;
        private SpyBranch _root;
        private SpyNode _child1;
        private SpyNode _child2;


        [SetUp]
        public void Setup()
        {
            // 1. Setup Infrastructure
            var commandBus = new CommandBus(new ServiceProviderCommandHandlerResolver(new SimpleServiceProvider()));
            _repository = new InMemoryGameRepository();

            // Initialize Repository with Empty State
            _repository.SaveGameState(global::TurnForge.Engine.Entities.GameState.Empty());

            var stubBoardFactory = new StubBoardFactory();
            _runtime = new GameEngineRuntime(commandBus, _repository, new TurnForgeOrchestrator(), new ConsoleLogger(), stubBoardFactory);

            // 2. Build FSM Tree: Root -> Child1 -> Child2
            // Use Builder to ensure correct linking
            var builder = new GameFlowBuilder();
            var nodeRoot = builder.AddRoot<SpyBranch>("Root", r =>
            {
                r.AddLeaf<SpyNode>("Child1");
                r.AddLeaf<SpyNode>("Child2");
            }).Build();

            // Extract references from built tree to assert on them
            // SpyNode instances are created by the builder via Activator.CreateInstance,
            // so we need to traverse the built tree to get the actual instances.
            var systemRoot = (TurnForge.Engine.Core.Fsm.SystemNodes.SystemRootNode)nodeRoot;
            _root = (SpyBranch)systemRoot.Children.Last();
            _child1 = (SpyNode)((BranchNode)_root).FirstChild;
            _child2 = (SpyNode)_child1.NextSibling;

            // 3. Init Controller starting at ROOT
            // Note: FsmController constructor automatically finds the first leaf if initialized with a BranchNode
            _controller = new FsmController(_root, _root.Id);
            _runtime.SetFsmController(_controller);

            // Sync repository with controller's current state (which auto-navigated to first leaf)
            var initialState = _repository.LoadGameState().WithCurrentStateId(_controller.CurrentStateId);
            _repository.SaveGameState(initialState);
        }

        [Test]
        public void MoveForward_UpdatesCurrentStateId()
        {
            // Arrange
            // Controller automatically starts at first leaf (Child1) when initialized with Root branch
            var initialState = _repository.LoadGameState();
            Assert.That(initialState.CurrentStateId, Is.EqualTo(_child1.Id), "Initial state should be Child1 (first leaf)");

            // Act: Request Move Forward (Child1 -> Child2)
            var newState = _controller.MoveForwardRequest(initialState);
            _repository.SaveGameState(newState.State);

            // Assert
            var loadedState = _repository.LoadGameState();
            Assert.That(loadedState.CurrentStateId, Is.EqualTo(_child2.Id), "State should move from Child1 to Child2");
        }

        [Test]
        public void MoveForward_TriggersOnStart_OnNewNode()
        {
            // Arrange
            var state = _repository.LoadGameState();
            Assert.That(state.CurrentStateId, Is.EqualTo(_child1.Id), "Should start at Child1");

            // Act: Move from Child1 to Child2
            var newState = _controller.MoveForwardRequest(state);
            _repository.SaveGameState(newState.State);

            // Assert
            Assert.That(_child2.OnStartCalled, Is.True, "Child2 OnStart should be called");
            Assert.That(_child1.OnEndCalled, Is.True, "Child1 OnEnd should be called (as we are leaving it to go to child2)");
        }

        [Test]
        public void SerialTransitions_Child1_Child2()
        {
            // Initial state is Child1 (auto-navigated from Root)
            Assert.That(_repository.LoadGameState().CurrentStateId, Is.EqualTo(_child1.Id));

            // 1. Child1 -> Child2
            var state2 = _controller.MoveForwardRequest(_repository.LoadGameState());
            _repository.SaveGameState(state2.State);

            Assert.That(_repository.LoadGameState().CurrentStateId, Is.EqualTo(_child2.Id));
            Assert.That(_child2.OnStartCalled, Is.True);
            Assert.That(_child1.OnEndCalled, Is.True);
        }
        [Test]
        public void SendCommand_WhenNodeRequestsTransition_TriggersMoveForward()
        {
            // Register a dummy command handler
            var services = new SimpleServiceProvider();
            services.Register<ICommandHandler<startFsmCommand>>(sp => new startFsmHandler());

            // Re-setup runtime with this registry
            var resolver = new ServiceProviderCommandHandlerResolver(services);
            var commandBus = new CommandBus(resolver);
            var stubBoardFactory = new StubBoardFactory();
            _runtime = new GameEngineRuntime(commandBus, _repository, new TurnForgeOrchestrator(), new ConsoleLogger(), stubBoardFactory);

            // The FSM only processes commands when the current state is a LeafNode.
            // BranchNodes cannot handle commands directly. 
            // Therefore, we need to start at Child1 (a LeafNode) to test command handling.
            _controller = new FsmController(_root, _child1.Id);
            _runtime.SetFsmController(_controller);
            _repository.SaveGameState(_repository.LoadGameState().WithCurrentStateId(_child1.Id));

            // Configure Child1 to request a transition when a command is executed
            _child1.ShouldTransitionOnCommand = true;
            _child1.AddAllowedCommand<startFsmCommand>();

            // Act: Execute command on Child1
            var result = _runtime.ExecuteCommand(new startFsmCommand()).Result;
            Assert.That(result.Success, Is.True);

            // Assert: Should transition from Child1 to Child2
            var newState = _repository.LoadGameState();
            Assert.That(newState.CurrentStateId, Is.EqualTo(_child2.Id), "Should move from Child1 to Child2 on command");
        }

        public record startFsmCommand : ICommand
        {
            public System.Type CommandType => typeof(startFsmCommand);
        }
        
        public class startFsmHandler : ICommandHandler<startFsmCommand>
        {
            public CommandResult Handle(startFsmCommand command) => CommandResult.Ok(System.Array.Empty<IDecision>());
        }
    }
}
