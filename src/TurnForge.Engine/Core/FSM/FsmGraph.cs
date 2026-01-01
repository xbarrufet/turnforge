using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm;

/// <summary>
/// FSM Graph that holds nodes and manages transitions (FSM 2.0).
/// 
/// Execution flow:
/// 1. ProcessFlow() runs OnEntry workflows and auto-transitions
/// 2. Check IsCompleted() on current node
/// 3. If completed, transition to GetNextNode()
/// </summary>
public class FsmGraph
{
    private readonly Dictionary<NodeId, BaseFsmNode> _nodes = new();
    private readonly IServiceProvider? _services;
    private readonly IGameLogger? _logger;
    
    public BaseFsmNode? CurrentNode { get; private set; }
    public BaseFsmNode? RootNode { get; private set; }
    
    /// <summary>
    /// Constructor accepting IFsmNode for backward compatibility with GameEngineFactory.
    /// </summary>
    public FsmGraph(IFsmNode? rootNode, IServiceProvider? services, IGameLogger? logger)
    {
        _services = services;
        _logger = logger;
        
        if (rootNode is BaseFsmNode baseNode)
        {
            SetRoot(baseNode);
        }
    }
    
    /// <summary>
    /// Simple constructor for tests.
    /// </summary>
    public FsmGraph() : this(null, null, null) { }
    
    /// <summary>
    /// Register a node in the graph.
    /// </summary>
    public void AddNode(BaseFsmNode node)
    {
        _nodes[node.Id] = node;
    }
    
    /// <summary>
    /// Set the root node (entry point).
    /// </summary>
    public void SetRoot(BaseFsmNode node)
    {
        AddNode(node);
        RootNode = node;
        CurrentNode = node;
    }

    /// <summary>
    /// Retrieve a node by ID. Useful for graph introspection.
    /// </summary>
    public BaseFsmNode? GetNode(NodeId id)
    {
        return _nodes.TryGetValue(id, out var node) ? node : null;
    }

    /// <summary>
    /// Reset the FSM to the initial state (RootNode).
    /// </summary>
    public void Reset()
    {
        if (RootNode != null)
        {
            CurrentNode = RootNode;
        }
    }
    
    /// <summary>
    /// Process FSM flow: execute OnEntry workflows and auto-transitions.
    /// This is the main entry point called by GameEngineRuntime.
    /// </summary>
    public FsmFlowResult ProcessFlow(GameState state)
    {
        if (CurrentNode == null)
        {
            return FsmFlowResult.NoChange(state);
        }
        
        var currentState = state;
        var events = new List<IGameEvent>();
        var isGameOver = false;
        
        // Execute OnEntry system workflows for current node
        currentState = ExecuteOnEntryActions(CurrentNode, currentState, events);
        
        // Check for auto-transitions
        while (CurrentNode != null && CurrentNode.IsCompleted(currentState))
        {
            var nextNode = CurrentNode.GetNextNode(currentState);
            
            if (nextNode == null)
            {
                // No next node = end of game
                isGameOver = true;
                _logger?.LogInfo($"FSM: No next node from {CurrentNode.Name}, game over.");
                break;
            }
            
            _logger?.LogInfo($"FSM: Transition {CurrentNode.Name} → {nextNode.Name}");
            CurrentNode = nextNode;
            
            // Execute OnEntry for new node
            currentState = ExecuteOnEntryActions(CurrentNode, currentState, events);
        }
        
        return new FsmFlowResult(currentState, events.AsReadOnly(), isGameOver);
    }
    
    private GameState ExecuteOnEntryActions(BaseFsmNode node, GameState state, List<IGameEvent> events)
    {
        var currentState = state;
        
        foreach (var workflow in node.OnEntryActions)
        {
            var context = new SystemActionContext(currentState);
            
            // Execute workflow nodes directly (system workflows don't suspend)
            var currentNode = workflow.StartNode;
            while (currentNode != null)
            {
                var result = currentNode.Execute(context);
                
                if (result.Status == ActionStatus.Failed)
                {
                    _logger?.LogError($"FSM OnEntry workflow failed: {result.ErrorMessage}");
                    break;
                }
                
                // Move to next node
                if (currentNode is Action.Builders.ILinkableNode linkable)
                {
                    currentNode = linkable.NextNode;
                }
                else
                {
                    break;
                }
            }
            
            // Commit overlay changes
            currentState = context.Overlay.Commit();
        }
        
        return currentState;
    }
    
    /// <summary>
    /// Check if a command is allowed in current node.
    /// </summary>
    public bool IsCommandAllowed(Type commandType)
    {
        return CurrentNode?.IsCommandAllowed(commandType) ?? false;
    }
}

/// <summary>
/// Simple context for system workflows (immediate completion).
/// </summary>
public sealed class SystemActionContext : ActionContext
{
    public SystemActionContext(GameState initialState)
    {
        InitializeState(initialState);
    }
    
    public override object? GetResult() => null;
}
