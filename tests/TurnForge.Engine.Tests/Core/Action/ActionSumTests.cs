using NUnit.Framework;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Core.Action;

[TestFixture]
public class ActionSumTests
{
    /// <summary>
    /// Test workflow with 2 nodes expecting numeric inputs.
    /// Result = input1 + input2
    /// </summary>
    [Test]
    public void Action_TwoNumericInputs_ReturnsSumOfBoth()
    {
        // Arrange
        var node2 = new SumInputNode("Node2", null);
        var node1 = new SumInputNode("Node1", node2);
        
        var workflow = new SumAction(node1);
        var context = new SumActionContext();
        context.InitializeState(GameState.Empty());
        
        var orchestrator = new ActionOrchestrator();
        
        // Act - Start workflow (suspends at node1)
        orchestrator.StartAction(workflow, context);
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Suspended));
        
        // Submit first input: 5
        var workflowId = Guid.Parse(workflow.Id.Value);
        orchestrator.SubmitInput(workflowId, new NumericInput(5));
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Suspended)); // Waiting at node2
        
        // Submit second input: 3
        orchestrator.SubmitInput(workflowId, new NumericInput(3));
        
        // Assert
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Completed));
        Assert.That(context.GetResult(), Is.EqualTo(8)); // 5 + 3
    }
    
    /// <summary>
    /// Test sending extra input to node2 before the expected input.
    /// Should sum all 3 inputs: initial + extra + expected
    /// </summary>
    [Test]
    public void Action_ExtraInputBeforeExpected_SumsAllThree()
    {
        // Arrange
        var node2 = new SumInputNode("Node2", null);
        var node1 = new SumInputNode("Node1", node2);
        
        var workflow = new SumAction(node1);
        var context = new SumActionContext();
        context.InitializeState(GameState.Empty());
        
        var orchestrator = new ActionOrchestrator();
        
        // Act - Start workflow
        orchestrator.StartAction(workflow, context);
        var workflowId = Guid.Parse(workflow.Id.Value);
        
        // Submit first input: 5
        orchestrator.SubmitInput(workflowId, new NumericInput(5));
        
        // Now at node2, but submit 2 inputs (extra + expected)
        // Since EnqueueInput adds to queue, we can add both before processing
        context.EnqueueInput(new NumericInput(10)); // Extra
        orchestrator.SubmitInput(workflowId, new NumericInput(3)); // Expected (this triggers execution)
        
        // Assert - Should have consumed all inputs and summed them
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Completed));
        // The SumInputNode consumes ALL available NumericInputs
        Assert.That(context.GetResult(), Is.EqualTo(18)); // 5 + 10 + 3
    }
    
    // =============================
    // Test Infrastructure
    // =============================
    
    /// <summary>
    /// Input that carries a numeric value.
    /// </summary>
    private record NumericInput(int Value) : IActionInput;
    
    /// <summary>
    /// Node that waits for numeric input and adds it to running sum.
    /// </summary>
    private class SumInputNode : INode
    {
        public NodeId Id { get; }
        public INode? NextNode { get; set; }
        
        public SumInputNode(string id, INode? next)
        {
            Id = new NodeId(id);
            NextNode = next;
        }
        
        public ActionStepResult Execute(ActionContext context)
        {
            // Check for inputs
            if (!context.HasInput<NumericInput>())
            {
                // Suspend and wait for input
                return ActionStepResult.Suspend("Waiting for numeric input", typeof(NumericInput));
            }
            
            // Consume ALL available NumericInputs
            var total = 0;
            while (context.HasInput<NumericInput>())
            {
                var input = context.ConsumeInput<NumericInput>();
                if (input != null)
                {
                    total += input.Value;
                }
            }
            
            // Add to running sum in context
            var currentSum = context.TryGet<int>("Sum", out var s) ? s : 0;
            context.Set("Sum", currentSum + total);
            
            return ActionStepResult.Success();
        }
    }
    
    /// <summary>
    /// Simple workflow with start node.
    /// </summary>
    private class SumAction : IAction
    {
        private readonly INode _startNode;
        
        public ActionId Id { get; } = new ActionId(Guid.NewGuid().ToString());
        public INode StartNode => _startNode;
        public IReadOnlyCollection<IReaction> GlobalReactions => Array.Empty<IReaction>();
        
        public SumAction(INode startNode)
        {
            _startNode = startNode;
        }
        
        public INode GetNode(NodeId nodeId)
        {
            var current = _startNode;
            while (current != null)
            {
                if (current.Id == nodeId) return current;
                current = current.NextNode;
            }
            throw new KeyNotFoundException($"Node {nodeId} not found");
        }
    }
    
    /// <summary>
    /// Context that returns the sum as result.
    /// </summary>
    private class SumActionContext : ActionContext
    {
        public override object? GetResult()
        {
            return TryGet<int>("Sum", out var s) ? s : 0;
        }
    }
    
    /// <summary>
    /// Test that if both inputs are provided at once, workflow completes without waiting.
    /// Given: Action with 2 steps, each expects 1 numeric input
    /// When: Both numbers are submitted before first execution
    /// Then: Action completes without waiting for second submit
    /// </summary>
    [Test]
    public void Action_BothInputsPreloaded_CompletesImmediately()
    {
        // Arrange - Use SingleInputNode that only consumes ONE input
        var node2 = new SingleInputNode("Node2", null);
        var node1 = new SingleInputNode("Node1", node2);
        
        var workflow = new SumAction(node1);
        var context = new SumActionContext();
        context.InitializeState(GameState.Empty());
        
        var orchestrator = new ActionOrchestrator();
        
        // Pre-load BOTH inputs BEFORE starting
        context.EnqueueInput(new NumericInput(5)); // For node1
        context.EnqueueInput(new NumericInput(3)); // For node2 (pre-loaded)
        
        // Act - Start workflow
        orchestrator.StartAction(workflow, context);
        
        // Assert - Should complete immediately since both inputs are available
        Assert.That(context.Status, Is.EqualTo(ActionStatus.Completed), 
            "Action should complete when all inputs are pre-loaded");
        Assert.That(context.GetResult(), Is.EqualTo(8)); // 5 + 3
    }
    
    /// <summary>
    /// Node that consumes exactly ONE input and adds to sum.
    /// Leaves remaining inputs in queue for next node.
    /// </summary>
    private class SingleInputNode : INode
    {
        public NodeId Id { get; }
        public INode? NextNode { get; set; }
        
        public SingleInputNode(string id, INode? next)
        {
            Id = new NodeId(id);
            NextNode = next;
        }
        
        public ActionStepResult Execute(ActionContext context)
        {
            if (!context.HasInput<NumericInput>())
            {
                return ActionStepResult.Suspend("Waiting for numeric input", typeof(NumericInput));
            }
            
            // Consume ONLY ONE input
            var input = context.ConsumeInput<NumericInput>();
            if (input != null)
            {
                var currentSum = context.TryGet<int>("Sum", out var s) ? s : 0;
                context.Set("Sum", currentSum + input.Value);
            }
            
            return ActionStepResult.Success();
        }
    }
}
