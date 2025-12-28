using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Core.Workflow;

// ============================================================================
// TurnForge.Engine – WorkflowContext
// ============================================================================

/// <summary>
/// WorkflowContext is a temporary, mutable container that exists
/// only during the execution of a workflow.
///
/// It is NOT the game state.
/// It is NOT persisted.
/// It is NOT shared between workflows.
///
/// Its purpose is to:
/// - hold execution-scoped data
/// - carry information between nodes
/// - support suspension and resumption
/// - maintain a WORKING COPY of the state that decisions apply to immediately
/// </summary>
public abstract class WorkflowContext
{
    // --------------------------------------------------------------------
    // Execution metadata
    // --------------------------------------------------------------------

    public WorkflowExecutionId ExecutionId { get; }

    public WorkflowStatus Status { get; internal set; }

    /// <summary>
    /// The ID of the node currently being executed (or where suspension occurred).
    /// </summary>
    public NodeId? CurrentNodeId { get; internal set; }

    protected WorkflowContext()
    {
        ExecutionId = WorkflowExecutionId.New();
        Status = WorkflowStatus.NotStarted;
    }

    // --------------------------------------------------------------------
    // Arbitrary workflow-scoped data
    // --------------------------------------------------------------------

    private readonly Dictionary<string, object> _data = new();

    public bool Has(string key)
        => _data.ContainsKey(key);

    public T Get<T>(string key)
    {
        if (!_data.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"WorkflowContext key '{key}' not found.");

        return (T)value;
    }

    public void Set<T>(string key, T value)
    {
        _data[key] = value!;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (_data.TryGetValue(key, out var obj) && obj is T typed)
        {
            value = typed;
            return true;
        }

        value = default!;
        return false;
    }

    public void Remove(string key)
        => _data.Remove(key);

    // --------------------------------------------------------------------
    // Diagnostics / tracing support
    // --------------------------------------------------------------------

    private readonly List<NodeTransition> _transitions = new();

    public IReadOnlyList<NodeTransition> Transitions => _transitions;

    internal void RecordTransition(NodeId from, NodeId to)
        => _transitions.Add(new NodeTransition(from, to));

    // --------------------------------------------------------------------
    // Navigation Stack (Nested Workflows)
    // --------------------------------------------------------------------

    private readonly Stack<WorkflowFrame> _navigationStack = new();

    public IReadOnlyCollection<WorkflowFrame> NavigationStack => _navigationStack;

    internal void PushFrame(WorkflowId workflowId, NodeId startNodeId, ReactionId? causingReactionId = null)
    {
        _navigationStack.Push(new WorkflowFrame(workflowId, startNodeId, causingReactionId));
        CurrentNodeId = startNodeId;
    }

    public WorkflowFrame PeekFrame() => _navigationStack.Peek();

    internal void PopFrame()
    {
        if (_navigationStack.Count > 0)
        {
            _navigationStack.Pop();
            if (_navigationStack.Count > 0)
            {
                CurrentNodeId = _navigationStack.Peek().CurrentNodeId;
            }
            else
            {
                CurrentNodeId = null;
            }
        }
    }

    internal void UpdateCurrentNode(NodeId nodeId)
    {
        CurrentNodeId = nodeId;
        if (_navigationStack.Count > 0)
        {
            var currentFrame = _navigationStack.Pop();
            _navigationStack.Push(currentFrame with { CurrentNodeId = nodeId });
        }
    }

    // --------------------------------------------------------------------
    // Working State (Immediate Apply)
    // --------------------------------------------------------------------
    
    private GameState? _workingState;
    private readonly List<IDecision> _appliedDecisions = new();

    /// <summary>
    /// The current working state. Decisions are applied immediately to this.
    /// </summary>
    public GameState State => _workingState ?? throw new InvalidOperationException("Working state not initialized.");

    /// <summary>
    /// History of all decisions applied during this workflow execution.
    /// Used for logging, debugging, and UI animation sequencing.
    /// </summary>
    public IReadOnlyList<IDecision> Decisions => _appliedDecisions.AsReadOnly();

    /// <summary>
    /// Initialize the working state from a base state copy.
    /// </summary>
    internal void InitializeState(GameState baseState)
    {
        _workingState = baseState;
    }

    /// <summary>
    /// Record and immediately apply a decision to the working state.
    /// </summary>
    public void RecordDecision(IDecision decision)
    {
        ArgumentNullException.ThrowIfNull(decision);
        
        // Log the decision
        _appliedDecisions.Add(decision);
        
        // Apply immediately to working state
        if (_workingState != null)
        {
            _workingState = decision.Apply(_workingState);
        }
    }

    /// <summary>
    /// Get the current state (same as State property).
    /// Kept for backward compatibility during migration.
    /// </summary>
    public GameState GetProjectedState() => State;

    // --------------------------------------------------------------------
    // Events
    // --------------------------------------------------------------------
    
    private readonly Queue<IWorkflowEvent> _pendingEvents = new();

    public bool HasPendingEvents => _pendingEvents.Count > 0;
    public IEnumerable<IWorkflowEvent> PendingEvents => _pendingEvents;
    
    public void AddEvent(IWorkflowEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _pendingEvents.Enqueue(domainEvent);
    }

    public IWorkflowEvent DequeueEvent()
    {
        return _pendingEvents.Dequeue();
    }
    
    internal void ClearEvents()
    {
        _pendingEvents.Clear();
    }
}

/// <summary>
/// Represents a snapshot of execution pointer within a specific workflow.
/// </summary>
public readonly record struct WorkflowFrame(WorkflowId WorkflowId, NodeId CurrentNodeId, ReactionId? CausingReactionId = null);
