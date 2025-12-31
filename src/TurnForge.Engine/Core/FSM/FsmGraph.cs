using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Core.Fsm;

/// <summary>
/// FSM 2.0 Graph Controller.
/// Manages game flow using a graph of IFsmNode.
/// Executes system workflows and resolvers on node entry and handles transitions.
/// </summary>
public class FsmGraph
{
    private readonly IFsmNode _rootNode;
    private readonly IServiceProvider _services;
    private readonly IGameLogger? _logger;
    private readonly WorkflowOrchestrator _workflowOrchestrator;
    
    private IFsmNode _currentNode;
    
    public IFsmNode CurrentNode => _currentNode;
    public bool IsGameOver { get; private set; }
    
    public FsmGraph(IFsmNode rootNode, IServiceProvider services, IGameLogger? logger = null)
    {
        _rootNode = rootNode;
        _services = services;
        _logger = logger;
        _currentNode = rootNode;
        _workflowOrchestrator = new WorkflowOrchestrator();
    }
    
    /// <summary>
    /// Enter the initial node and execute its entry logic.
    /// Call this once when starting the game.
    /// </summary>
    public FsmGraphResult Initialize(GameState state)
    {
        _logger?.LogInfo($"[FsmGraph] Initializing at node: {_currentNode.Name}");
        return ExecuteNodeEntry(state);
    }
    
    /// <summary>
    /// Process the FSM flow. Transitions nodes as needed.
    /// Returns updated state and events from resolvers.
    /// </summary>
    public FsmGraphResult ProcessFlow(GameState state)
    {
        var currentState = state;
        var allEvents = new List<IGameEvent>();
        
        int loopGuard = 0;
        const int MaxIterations = 100;
        
        while (loopGuard++ < MaxIterations)
        {
            // Check if current node is completed
            if (_currentNode.IsCompleted(currentState))
            {
                // Get next node
                var nextNode = _currentNode.GetNextNode(currentState);
                
                if (nextNode == null)
                {
                    // Terminal node - game over
                    IsGameOver = true;
                    _logger?.LogInfo("[FsmGraph] Game Over - no next node");
                    return new FsmGraphResult(currentState, allEvents, true);
                }
                
                // Transition to next node
                _currentNode = nextNode;
                _logger?.LogInfo($"[FsmGraph] Transition to: {_currentNode.Name}");
                
                // Execute entry logic (workflows + resolvers)
                var entryResult = ExecuteNodeEntry(currentState);
                currentState = entryResult.State;
                allEvents.AddRange(entryResult.Events);
                
                // Continue processing (next node might also auto-complete)
                continue;
            }
            else
            {
                // Node not completed - waiting for player action
                return new FsmGraphResult(currentState, allEvents, false);
            }
        }
        
        _logger?.LogError("[FsmGraph] Infinite loop detected");
        return new FsmGraphResult(currentState, allEvents, false);
    }
    
    /// <summary>
    /// Execute system workflows and resolvers when entering a node.
    /// Workflows execute first, then legacy resolvers.
    /// </summary>
    private FsmGraphResult ExecuteNodeEntry(GameState state)
    {
        var currentState = state;
        var allEvents = new List<IGameEvent>();
        
        // 1. Execute OnEntry Workflows (System Workflows - no user input)
        foreach (var workflow in _currentNode.OnEntryWorkflows)
        {
            _logger?.LogInfo($"[FsmGraph] Executing OnEntry workflow: {workflow.Id.Value}");
            
            // Create a system context for this workflow
            var context = new SystemWorkflowContext(currentState);
            
            _workflowOrchestrator.StartWorkflow(workflow, context);
            
            // System workflows should complete immediately
            // If suspended, that's an error - system workflows don't wait for input
            if (context.Status == ValueObjects.WorkflowStatus.Suspended)
            {
                _logger?.LogError($"[FsmGraph] System workflow {workflow.Id.Value} suspended - this is not allowed");
            }
            
            // Update state from workflow context
            currentState = context.State ?? currentState;
        }
        
        // 2. Execute legacy Resolvers (for backward compatibility)
        var resolverContext = new ResolverContext(currentState, _services);
        
        foreach (var resolver in _currentNode.Resolvers)
        {
            _logger?.LogInfo($"[FsmGraph] Executing resolver: {resolver.Name}");
            
            var result = resolver.Resolve(resolverContext);
            currentState = result.State;
            allEvents.AddRange(result.Events);
            
            // Update context with new state for next resolver
            resolverContext = resolverContext with { State = currentState };
        }
        
        return new FsmGraphResult(currentState, allEvents, false);
    }
    
    /// <summary>
    /// Check if a command is allowed in the current node.
    /// </summary>
    public bool IsCommandAllowed(Type commandType)
    {
        return _currentNode.IsCommandAllowed(commandType);
    }
    
    /// <summary>
    /// Get list of allowed commands for current node.
    /// </summary>
    public IReadOnlyList<Type> GetAllowedCommands()
    {
        return _currentNode.AllowedCommands;
    }
}

/// <summary>
/// Result of FSM graph processing.
/// </summary>
public record FsmGraphResult(
    GameState State,
    IReadOnlyList<IGameEvent> Events,
    bool IsGameOver
);

/// <summary>
/// Simple workflow context for system workflows.
/// System workflows update GameState directly.
/// </summary>
internal class SystemWorkflowContext : WorkflowContext
{
    public new GameState? State { get; private set; }
    
    public SystemWorkflowContext(GameState state)
    {
        State = state;
    }
    
    public override object? GetResult() => State;
    
    public new void UpdateState(GameState state)
    {
        State = state;
    }
}
