using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Nodes;

/// <summary>
/// Single unified FSM node. Behavior is determined by configuration:
/// - OnEntryWorkflows: System workflows to execute on entry (preferred)
/// - Resolvers: Legacy resolvers for simple actions
/// - AllowedCommands: Interactive (waits for input) or passthrough
/// - GetNextNode: Where to transition after completion
/// - IsCompleted: When to transition
/// </summary>
public class FsmNode : IFsmNode
{
    private readonly List<INodeResolver> _resolvers = new();
    private readonly List<IWorkflow> _onEntryWorkflows = new();
    private readonly List<Type> _allowedCommands = new();
    private Func<GameState, IFsmNode?>? _nextNodeFunc;
    private Func<GameState, bool>? _isCompletedFunc;
    
    public NodeId Id { get; }
    public string Name { get; }
    
    public IReadOnlyList<INodeResolver> Resolvers => _resolvers;
    public IReadOnlyList<IWorkflow> OnEntryWorkflows => _onEntryWorkflows;
    public IReadOnlyList<Type> AllowedCommands => _allowedCommands;
    
    public FsmNode(string name)
    {
        Id = new NodeId(Guid.NewGuid().ToString());
        Name = name;
    }
    
    public FsmNode(NodeId id, string name)
    {
        Id = id;
        Name = name;
    }
    
    // --- Fluent Configuration Methods ---
    
    /// <summary>
    /// Add a system workflow to execute on node entry.
    /// Workflows run automatically without user input.
    /// </summary>
    public FsmNode OnEntry(IWorkflow workflow)
    {
        _onEntryWorkflows.Add(workflow);
        return this;
    }
    
    /// <summary>
    /// Add multiple system workflows to execute on node entry.
    /// </summary>
    public FsmNode OnEntry(params IWorkflow[] workflows)
    {
        _onEntryWorkflows.AddRange(workflows);
        return this;
    }
    
    /// <summary>
    /// Add a resolver to execute on node entry.
    /// [Deprecated] Use OnEntry(IWorkflow) for new code.
    /// </summary>
    public FsmNode WithResolver(INodeResolver resolver)
    {
        _resolvers.Add(resolver);
        return this;
    }
    
    /// <summary>
    /// Add a resolver by type (requires DI registration).
    /// [Deprecated] Use OnEntry(IWorkflow) for new code.
    /// </summary>
    public FsmNode WithResolver<TResolver>() where TResolver : INodeResolver, new()
    {
        _resolvers.Add(new TResolver());
        return this;
    }
    
    /// <summary>
    /// Set allowed commands for this node.
    /// Empty = passthrough node (no user interaction).
    /// </summary>
    public FsmNode WithAllowedCommands(params Type[] commands)
    {
        _allowedCommands.Clear();
        _allowedCommands.AddRange(commands);
        return this;
    }
    
    /// <summary>
    /// Set dynamic next node function.
    /// </summary>
    public FsmNode WithNextNode(Func<GameState, IFsmNode?> nextNodeFunc)
    {
        _nextNodeFunc = nextNodeFunc;
        return this;
    }
    
    /// <summary>
    /// Set static next node (always transitions to same node).
    /// </summary>
    public FsmNode WithNextNode(IFsmNode nextNode)
    {
        _nextNodeFunc = _ => nextNode;
        return this;
    }
    
    /// <summary>
    /// Set completion condition.
    /// </summary>
    public FsmNode WithCompletionCondition(Func<GameState, bool> isCompletedFunc)
    {
        _isCompletedFunc = isCompletedFunc;
        return this;
    }
    
    // --- IFsmNode Implementation ---
    
    public IFsmNode? GetNextNode(GameState state)
    {
        return _nextNodeFunc?.Invoke(state);
    }
    
    public bool IsCommandAllowed(Type commandType)
    {
        return _allowedCommands.Contains(commandType);
    }
    
    public bool IsCompleted(GameState state)
    {
        // Default: completed when no allowed commands (passthrough)
        // or when custom condition is met
        if (_isCompletedFunc != null)
        {
            return _isCompletedFunc(state);
        }
        
        // Passthrough nodes complete immediately
        return _allowedCommands.Count == 0;
    }
}
