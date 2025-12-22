
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
using TurnForge.Engine.Core.Orchestrator; // Added
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
            var commandBus = new CommandBus(new GameLoop(), new ServiceProviderCommandHandlerResolver(new SimpleServiceProvider()));
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

            // Extract references from built tree to assert on them?
            // "SpyNode" instances are created by the builder via Activator.CreateInstance. 
            // So my `_child1` field above is NOT the one in the tree.
            // I need to traverse the built tree to get the actual instances.
            // Extract references from built tree to assert on them?
            // "SpyNode" instances are created by the builder via Activator.CreateInstance. 
            // So my `_child1` field above is NOT the one in the tree.
            // I need to traverse the built tree to get the actual instances.
            var systemRoot = (TurnForge.Engine.Core.Fsm.SystemNodes.SystemRootNode)nodeRoot;
            _root = (SpyBranch)systemRoot.Children.Last();
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
            var newState = _controller.MoveForwardRequest(initialState);
            _repository.SaveGameState(newState.State);

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
            var newState = _controller.MoveForwardRequest(state);
            _repository.SaveGameState(newState.State);

            // Assert
            Assert.That(_child1.OnStartCalled, Is.True, "Child1 OnStart should be called");
            Assert.That(_root.OnEndCalled, Is.True, "Root OnEnd should be called (as we are leaving it to go to child)");
        }

        [Test]
        public void SerialTransitions_Root_Child1_Child2()
        {
            // 1. Root -> Child1
            var state1 = _controller.MoveForwardRequest(_repository.LoadGameState());
            _repository.SaveGameState(state1.State);
            Assert.That(_repository.LoadGameState().CurrentStateId, Is.EqualTo(_child1.Id));

            // 2. Child1 -> Child2
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
            var commandBus = new CommandBus(new GameLoop(), resolver);
            var stubBoardFactory = new StubBoardFactory();
            _runtime = new GameEngineRuntime(commandBus, _repository, new TurnForgeOrchestrator(), new ConsoleLogger(), stubBoardFactory);

            // IMPORTANT: Configure the node to request transition!
            // This replaces the old "StartFSM" tag hack. The node itself must decide logic.
            // Since _child1 is a created instance via Activator in GameFlowBuilder, 
            // we must ensure we are modifying the instance IN THE CONTROLLER?
            // Actually, `_root` and `_child1` extracted in Setup ARE the instances used by Controller
            // because `builder.Build()` creates them, and `Flatten` stores them.
            // But wait, `builder.AddRoot` -> creates instances.
            // `_root = (SpyBranch)nodeRoot` -> Reference to same object.
            // BUT: `_root` is where we start.
            // The test expects transition from ROOT to CHILD1?
            // Test "Should move to first child".
            // So we need to set `_root.ShouldTransitionOnCommand = true`?
            // Because `_currentStateId` is `_root.Id`.
            // Controller delegates `HandleCommand` to `_nodesById[_currentStateId]` i.e. Root.
            // Root is `BranchNode`. Wait. `BranchNode` logic?
            // `FsmController.HandleCommand`: `if (_nodesById[_currentStateId] is LeafNode leaf)`.
            // Wait! `_root` is `SpyBranch` (BranchNode).
            // A BranchNode CANNOT execute commands directly in `FsmController` logic!
            // Line 137: `if (... is LeafNode leaf)`.
            // If current state is Branch, `HandleCommand` returns `false` (no transition).
            // This test seems flawed if it expects transition from a Branch via Command?
            // BUT `SpyBranch` does NOT inherit `LeafNode`.
            // `SpyNode` inherits `LeafNode`.
            // The FSM structure in Setup is `Root(Branch) -> Child1(Leaf)`.
            // If `CurrentStateId` IS `Root`, then `HandleCommand` does nothing.
            // Wait, previous test passed "WithStartFSMTag".
            // Because the HACK in Runtime forced `MoveForwardRequest` IGNORING the Node logic.
            // Now that we removed the hack, `HandleCommand` runs logic.
            // Logic says: Only LeafNodes handle commands.
            // So if `Root` is current state, it cannot handle commands.
            // BUT `MoveForwardRequest` moves INTO the branch's first child.
            // So `InitialState` must be a LEAF.
            // In the real app, `SystemRoot` has `InitialStateNode` (Leaf) as first child.
            // Does `FsmController` automatically enter the first leaf on startup?
            // No, `_currentStateId` is set to `initialId`.
            // If `initialId` is the Branch, we are stuck?
            // `MoveForwardRequest` logic: `GetNextStateId`.
            // If at Branch, `GetNextStateId` enters `FirstChild`.
            // So in the old system, `StartFSM` hack called `MoveForward` which entered the branch.
            // In the NEW standard system, we expect the user/system to start AT a Leaf?
            // Or `InitGameCommand` logic...
            // `InitialStateNode` IS a Leaf.
            // So `InitialStateNode` must be the `CurrentState` when `InitGameCommand` is sent.
            // In THIS test, `Root` is `SpyBranch`.
            // If we want to test command handling, `CurrentState` must be a Leaf.
            // So I should initialize `FsmController` pointing to `_child1`?
            // Or change `Root` to be a Leaf for this test?

            // Actually, the real system uses `SystemRoot` (Branch) -> `InitialState` (Leaf).
            // And `FsmController` is initialized with `initialId`.
            // Who determines `initialId`? The caller.
            // In `FsmTransitionTests.Setup`, `_controller = new FsmController(_root, _root.Id)`.
            // So it starts at Branch.
            // This is INVALID for command handling unless Branch handles commands (it doesn't).
            // So the test setup is creating an invalid state for standard command processing.
            // I should modify Setup (or this specific test) to start at a Leaf if testing commands.
            // OR change `SpyBranch` to `SpyNode` for Root? But `BranchBuilder` expects Branch.

            // I will modify the test to set CurrentState to `_child1` (Leaf) manually?
            // No, `_child1` -> `_child2` transition via Command?
            // That works.
            // I'll make the test verify `Child1 -> Child2` transition via Command.
            // Setup:
            _controller = new FsmController(_root, _child1.Id);
            _runtime.SetFsmController(_controller);
            _repository.SaveGameState(_repository.LoadGameState().WithCurrentStateId(_child1.Id));

            // Enable transition desire on Child1
            _child1.ShouldTransitionOnCommand = true;
            _child1.AddAllowedCommand<startFsmCommand>();

            // Act
            var result = _runtime.ExecuteCommand(new startFsmCommand()).Result;
            Assert.That(result.Success, Is.True);

            // Assert
            var newState = _repository.LoadGameState();
            Assert.That(newState.CurrentStateId, Is.EqualTo(_child2.Id), "Should move from Child1 to Child2 on command");
        }

        public record startFsmCommand : ICommand
        {
            public System.Type CommandType => typeof(startFsmCommand);
        }
        public class startFsmHandler : ICommandHandler<startFsmCommand>
        {
            public CommandResult Handle(startFsmCommand command) => CommandResult.Ok(System.Array.Empty<TurnForge.Engine.Entities.Decisions.Interfaces.IDecision>(), tags: "StartFSM");
        }
    }
}
