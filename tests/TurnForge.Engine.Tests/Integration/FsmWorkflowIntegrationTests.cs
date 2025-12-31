using NUnit.Framework;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Integration;

/// <summary>
/// Integration test combining FSM (game flow) with Workflow (command execution).
/// 
/// Structure:
/// - 1 Round with 2 Turns
/// - Each turn accepts ADD command (workflow that adds 1)
/// - Round counter starts at 0
/// - Game ends when counter reaches 10
/// </summary>
[TestFixture]
public class FsmWorkflowIntegrationTests
{
    [Test]
    public void FsmWithWorkflow_AddCommandsUntil10_GameEnds()
    {
        // Arrange - Create the game state tracker
        var gameCounter = 0;
        
        // Create FSM nodes
        FsmNode roundNode = null!;
        FsmNode turn1Node = null!;
        FsmNode turn2Node = null!;
        FsmNode endRoundNode = null!;
        
        roundNode = new FsmNode("Round")
            .WithResolver(new CounterResolver(() => gameCounter, v => gameCounter = v, "RoundStart"))
            .WithCompletionCondition(_ => true); // Pass through
            
        turn1Node = new FsmNode("Turn1")
            .WithAllowedCommands(typeof(AddCommand)) // Interactive - waits for command
            .WithCompletionCondition(_ => false); // Never auto-complete (waits for command processing)
            
        turn2Node = new FsmNode("Turn2")
            .WithAllowedCommands(typeof(AddCommand))
            .WithCompletionCondition(_ => false);
            
        endRoundNode = new FsmNode("EndRound")
            .WithCompletionCondition(_ => true); // Pass through
        
        // Wire transitions
        roundNode.WithNextNode(_ => turn1Node);
        turn1Node.WithNextNode(_ => turn2Node);
        turn2Node.WithNextNode(_ => endRoundNode);
        endRoundNode.WithNextNode(_ => 
        {
            // Game over when counter >= 10
            if (gameCounter >= 10) return null;
            return roundNode; // Next round
        });
        
        // Create FSM and workflow orchestrator
        var services = new SimpleTestServiceProvider();
        var fsm = new FsmGraph(roundNode, services, null);
        var workflowOrchestrator = new WorkflowOrchestrator();
        var state = GameState.Empty();
        
        // Act - Initialize FSM
        var result = fsm.Initialize(state);
        result = fsm.ProcessFlow(result.State);
        
        // Should be at Turn1, waiting for command
        Assert.That(fsm.CurrentNode.Name, Is.EqualTo("Turn1"));
        Assert.That(fsm.IsCommandAllowed(typeof(AddCommand)), Is.True);
        
        // Simulate game loop: Execute ADD commands until game ends
        int commandCount = 0;
        const int maxIterations = 20; // Safety limit
        
        while (!fsm.IsGameOver && commandCount < maxIterations)
        {
            // Create and execute ADD workflow
            var addWorkflow = new AddWorkflow(() => gameCounter, v => gameCounter = v);
            var context = new AddWorkflowContext();
            context.InitializeState(result.State);
            
            workflowOrchestrator.StartWorkflow(addWorkflow, context);
            commandCount++;
            
            // Mark current node as completed (command processed) and advance
            var currentNode = fsm.CurrentNode;
            
            // Manually advance by simulating completion
            // In real system, the command handler would mark this
            if (currentNode.Name == "Turn1")
            {
                // Force transition to Turn2
                turn1Node.WithCompletionCondition(_ => true);
                result = fsm.ProcessFlow(result.State);
                turn1Node.WithCompletionCondition(_ => false); // Reset
            }
            else if (currentNode.Name == "Turn2")
            {
                // Force transition to EndRound -> Round
                turn2Node.WithCompletionCondition(_ => true);
                result = fsm.ProcessFlow(result.State);
                turn2Node.WithCompletionCondition(_ => false); // Reset
            }
        }
        
        // Assert
        Assert.That(fsm.IsGameOver, Is.True, "Game should be over");
        Assert.That(gameCounter, Is.GreaterThanOrEqualTo(10), "Counter should be at least 10");
        Assert.That(commandCount, Is.EqualTo(10), "Should have taken exactly 10 ADD commands");
    }
    
    // =============================
    // Test Infrastructure
    // =============================
    
    /// <summary>
    /// Command that triggers ADD workflow.
    /// </summary>
    private record AddCommand : Engine.Commands.Interfaces.ICommand
    {
        public Engine.Commands.ValueObjects.CommandType CommandType => new("Add");
    }
    
    /// <summary>
    /// Resolver that tracks counter (used for debugging/logging).
    /// </summary>
    private class CounterResolver : INodeResolver
    {
        private readonly Func<int> _getCounter;
        private readonly Action<int> _setCounter;
        
        public string Name { get; }
        
        public CounterResolver(Func<int> getCounter, Action<int> setCounter, string name)
        {
            _getCounter = getCounter;
            _setCounter = setCounter;
            Name = name;
        }
        
        public ResolverResult Resolve(ResolverContext context)
        {
            // Just log, don't modify counter here
            Console.WriteLine($"[{Name}] Counter = {_getCounter()}");
            return ResolverResult.From(context.State);
        }
    }
    
    /// <summary>
    /// Workflow that adds 1 to the counter.
    /// </summary>
    private class AddWorkflow : IWorkflow
    {
        private readonly Func<int> _getCounter;
        private readonly Action<int> _setCounter;
        
        public WorkflowId Id { get; } = new WorkflowId(Guid.NewGuid().ToString());
        public INode StartNode { get; }
        public IReadOnlyCollection<IReaction> GlobalReactions => Array.Empty<IReaction>();
        
        public AddWorkflow(Func<int> getCounter, Action<int> setCounter)
        {
            _getCounter = getCounter;
            _setCounter = setCounter;
            StartNode = new AddNode(_getCounter, _setCounter);
        }
        
        public INode GetNode(NodeId nodeId ) => StartNode;
    }
    
    /// <summary>
    /// Workflow node that adds 1 to counter.
    /// </summary>
    private class AddNode : INode
    {
        private readonly Func<int> _getCounter;
        private readonly Action<int> _setCounter;
        
        public NodeId Id { get; } = new NodeId("add-node");
        public INode? NextNode { get; set; } = null;
        
        public AddNode(Func<int> getCounter, Action<int> setCounter)
        {
            _getCounter = getCounter;
            _setCounter = setCounter;
        }
        
        public WorkflowStepResult Execute(WorkflowContext context)
        {
            var current = _getCounter();
            _setCounter(current + 1);
            Console.WriteLine($"[AddNode] Counter: {current} -> {current + 1}");
            return WorkflowStepResult.Success();
        }
    }
    
    /// <summary>
    /// Context for ADD workflow.
    /// </summary>
    private class AddWorkflowContext : WorkflowContext
    {
        public override object? GetResult() => null;
    }
    
    private class SimpleTestServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
