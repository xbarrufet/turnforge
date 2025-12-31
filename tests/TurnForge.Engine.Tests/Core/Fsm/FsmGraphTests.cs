using NUnit.Framework;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Fsm.Nodes;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Tests.Core.Fsm;

[TestFixture]
public class FsmGraphTests
{
    /// <summary>
    /// Test FSM with: Round -> Turn1 -> Turn2 -> EndRound -> (loop or GameOver)
    /// After 2 rounds, game ends.
    /// </summary>
    [Test]
    public void FsmGraph_TwoRoundsWithTwoTurns_CompletesGame()
    {
        // Arrange - Create nodes
        var gameOverNode = new FsmNode("GameOver");
        
        FsmNode? roundNode = null;
        FsmNode? turn1Node = null;
        FsmNode? turn2Node = null;
        FsmNode? endRoundNode = null;
        
        // Track round count in state via custom flag
        var roundCount = 0;
        
        roundNode = new FsmNode("Round")
            .WithCompletionCondition(_ => true); // Always pass through
            
        turn1Node = new FsmNode("Turn1")
            .WithCompletionCondition(_ => true); // Auto complete for test
            
        turn2Node = new FsmNode("Turn2")
            .WithCompletionCondition(_ => true);
            
        endRoundNode = new FsmNode("EndRound")
            .WithCompletionCondition(_ => 
            {
                roundCount++;
                return true;
            });
        
        // Wire up transitions
        roundNode.WithNextNode(_ => turn1Node);
        turn1Node.WithNextNode(_ => turn2Node);
        turn2Node.WithNextNode(_ => endRoundNode);
        endRoundNode.WithNextNode(_ => 
        {
            // After 2 rounds, game over
            if (roundCount >= 2) return null; // null = game over
            return roundNode;
        });
        
        // Create FSM
        var services = new SimpleTestServiceProvider();
        var fsm = new FsmGraph(roundNode, services, null);
        var state = GameState.Empty();
        
        // Act - Initialize
        var initResult = fsm.Initialize(state);
        Assert.That(fsm.IsGameOver, Is.False);
        
        // Process until game over
        var result = fsm.ProcessFlow(initResult.State);
        
        // Assert
        Assert.That(fsm.IsGameOver, Is.True, "Game should be over after 2 rounds");
        Assert.That(roundCount, Is.EqualTo(2), "Should have completed exactly 2 rounds");
    }
    
    [Test]
    public void FsmGraph_InteractiveNode_SuspendsUntilCompleted()
    {
        // Arrange - Create interactive node that needs command
        var interactiveCompleted = false;
        
        var startNode = new FsmNode("Start")
            .WithCompletionCondition(_ => true);
            
        var interactiveNode = new FsmNode("WaitForPlayer")
            .WithAllowedCommands(typeof(TestCommand)) // Has commands = interactive
            .WithCompletionCondition(_ => interactiveCompleted);
            
        var endNode = new FsmNode("End")
            .WithCompletionCondition(_ => true);
        
        startNode.WithNextNode(_ => interactiveNode);
        interactiveNode.WithNextNode(_ => endNode);
        endNode.WithNextNode(_ => null); // Game over
        
        var services = new SimpleTestServiceProvider();
        var fsm = new FsmGraph(startNode, services, null);
        var state = GameState.Empty();
        
        // Act - Initialize and process
        var result = fsm.Initialize(state);
        result = fsm.ProcessFlow(result.State);
        
        // Assert - Should stop at interactive node
        Assert.That(fsm.CurrentNode.Name, Is.EqualTo("WaitForPlayer"));
        Assert.That(fsm.IsGameOver, Is.False);
        Assert.That(fsm.IsCommandAllowed(typeof(TestCommand)), Is.True);
        
        // Act - Mark as completed and process again
        interactiveCompleted = true;
        result = fsm.ProcessFlow(result.State);
        
        // Assert - Should reach game over
        Assert.That(fsm.IsGameOver, Is.True);
    }
    
    [Test]
    public void FsmGraph_Resolvers_ExecuteOnNodeEntry()
    {
        // Arrange
        var resolverExecuted = false;
        var testResolver = new TestResolver(() => resolverExecuted = true);
        
        var startNode = new FsmNode("Start")
            .WithResolver(testResolver)
            .WithCompletionCondition(_ => true);
            
        startNode.WithNextNode(_ => null);
        
        var services = new SimpleTestServiceProvider();
        var fsm = new FsmGraph(startNode, services, null);
        var state = GameState.Empty();
        
        // Act
        fsm.Initialize(state);
        
        // Assert
        Assert.That(resolverExecuted, Is.True, "Resolver should execute on node entry");
    }
    
    // Test helpers
    private record TestCommand : Engine.Commands.Interfaces.ICommand
    {
        public Engine.Commands.ValueObjects.CommandType CommandType => new("Test");
    }
    
    private class TestResolver : INodeResolver
    {
        private readonly Action _onResolve;
        public string Name => "TestResolver";
        
        public TestResolver(Action onResolve) => _onResolve = onResolve;
        
        public ResolverResult Resolve(ResolverContext context)
        {
            _onResolve();
            return ResolverResult.From(context.State);
        }
    }
    
    private class SimpleTestServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
