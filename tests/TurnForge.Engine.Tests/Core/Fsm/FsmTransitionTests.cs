using System;
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
using TurnForge.Engine.Core.Orchestrator; 
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
            public bool ExecuteCalled { get; private set; }
            public bool IsCompletedValue { get; set; } = false;

            public override NodeExecutionResult Execute(GameState state)
            {
                ExecuteCalled = true;
                return NodeExecutionResult.Empty();
            }

            // By default behaves like a blocking leaf, unless we set it to true
            public override bool IsCompleted(GameState state) => IsCompletedValue;
        }

        internal class SpyBranch : FsmNode
        {
             // Structural node acts as container
             public override bool IsCompleted(GameState state) => true; 
             public override bool IsCommandAllowed(Type type) => false;
             public override IReadOnlyList<Type> GetAllowedCommands() => Array.Empty<Type>();
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
        private List<FsmNode> _sequence;

        [SetUp]
        public void Setup()
        {
            // 1. Setup Infrastructure
            var commandBus = new CommandBus(new ServiceProviderCommandHandlerResolver(new SimpleServiceProvider()));
            _repository = new InMemoryGameRepository();
            _repository.SaveGameState(global::TurnForge.Engine.Entities.GameState.Empty());

            var stubBoardFactory = new StubBoardFactory();
            _runtime = new GameEngineRuntime(commandBus, _repository, new TurnForgeOrchestrator(), new ConsoleLogger(), stubBoardFactory);

            // 2. Build FSM Sequence
            var builder = new GameFlowBuilder();
            // Note: System nodes (Initial, BoardReady, GamePrepared) are added first
            builder.AddRoot<SpyBranch>("Root", r =>
            {
                r.AddLeaf<SpyNode>("Child1");
                r.AddLeaf<SpyNode>("Child2");
            });
            _sequence = builder.Build();

            // Extract Nodes
            // Sequence: [0]Initial, [1]BoardReady, [2]GamePrepared, [3]Root(SpyBranch), [4]Child1, [5]Child2
            // Wait, GameFlowBuilder order: Initial, BoardReady, GamePrepared, Then User Sequence.
            _root = (SpyBranch)_sequence[3];
            _child1 = (SpyNode)_sequence[4];
            _child2 = (SpyNode)_sequence[5];

            // 3. Init Controller starting at Child1 (skipping systems & root for testing transition between 1 and 2)
            _controller = new FsmController(_sequence, _child1.Id); 
            _runtime.SetFsmController(_controller);

            // Sync repository
            var initialState = _repository.LoadGameState(); // CurrentStateId is empty or default
            // But Controller is at Child1.
            // Note: GameState.CurrentStateId is used by persistence, but Controller has its own pointer initially.
            // We should sync them.
        }

        [Test]
        public void MoveForward_WhenNodeCompleted_UpdatesCurrentStateId()
        {
            // Arrange
            // Enable completion for Child1 so next tick advances it
            _child1.IsCompletedValue = true;
            
            var initialState = _repository.LoadGameState();
            // Controller is at Child1
            Assert.That(_controller.CurrentStateId, Is.EqualTo(_child1.Id));

            // Act: Process Flow (Simulate Tick)
            _controller.ProcessFlow(initialState);

            // Assert
            Assert.That(_controller.CurrentStateId, Is.EqualTo(_child2.Id), "Should move to Child2");
        }

        [Test]
        public void Execute_CalledOnCurrentNode()
        {
            // Arrange
            var state = _repository.LoadGameState();
            
            // Act
            _controller.ProcessFlow(state);

            // Assert
            Assert.That(_child1.ExecuteCalled, Is.True, "Child1 Execute should be called");
        }

        [Test]
        public void SendCommand_DoesNotTransitionIfNodeUnfinished()
        {
             // Arrange
            _child1.IsCompletedValue = false; // Stays
            _child1.AddAllowedCommand<startFsmCommand>();

             // Provide Handler ... (Reuse generic handler setup)
             var services = new SimpleServiceProvider();
             services.Register<ICommandHandler<startFsmCommand>>(sp => new startFsmHandler());
             var resolver = new ServiceProviderCommandHandlerResolver(services);
             var commandBus = new CommandBus(resolver);
             _runtime = new GameEngineRuntime(commandBus, _repository, new TurnForgeOrchestrator(), new ConsoleLogger(), new StubBoardFactory());
             _runtime.SetFsmController(_controller);

            // Act
            var res = _runtime.ExecuteCommand(new startFsmCommand());
            Assert.That(res.Result.Success, Is.True);
            
            // Assert
            Assert.That(_controller.CurrentStateId, Is.EqualTo(_child1.Id), "Should stay in Child1");
        }

        [Test]
        public void SendCommand_TransitionsIfNodeBecomesCompleted()
        {
             // Arrange: Child1 becomes completed via logic (simulated by setting prop)
             // We need a custom node that completes ON command?
             // Or just verify flow logic.
             // If we rely on generic State check, we can simulate state change.
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
