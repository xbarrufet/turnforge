using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action;

// ============================================================================
// TurnForge.Engine – ActionContext
// ============================================================================

/// <summary>
/// ActionContext is a temporary, mutable container that exists
/// only during the execution of an action.
///
/// It is NOT the game state.
/// It is NOT persisted.
/// It is NOT shared between actions.
///
/// Its purpose is to:
/// - hold execution-scoped data
/// - carry information between nodes
/// - support suspension and resumption
/// 
/// Note: State changes are recorded via GameState.RecordOverlayOperation(),
/// not via ActionContext. The context only holds workflow-scoped data.
/// </summary>
public class ActionContext
{
    private readonly Queue<IActionInput> _inputQueue = new();
    private readonly List<IActionInput> _history = new();
    private readonly Dictionary<string, object> _data = new();

    public ActionContext()
    {
        ExecutionId = ActionExecutionId.Empty;
        Status = ActionStatus.NotStarted;
    }

    /// <summary>
    /// Unique identifier for this action execution instance.
    /// Assigned by ActionOrchestrator when starting the action.
    /// </summary>
    public ActionExecutionId ExecutionId { get; private set; }

    internal void SetExecutionId(ActionExecutionId executionId)
    {
        ExecutionId = executionId;
    }

    /// <summary>
    /// Current execution status.
    /// </summary>
    public ActionStatus Status { get; protected set; }

    /// <summary>
    /// Reason for failure if Status is Failed.
    /// </summary>
    public string? ErrorMessage { get; protected set; }

    // --------------------------------------------------------------------
    // Status Management (internal, called by orchestrator)
    // --------------------------------------------------------------------

    /// <summary>
    /// Updates the action status. Called by the orchestrator during execution.
    /// </summary>
    internal void UpdateStatus(ActionStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Updates the error message. Called by the orchestrator on failure.
    /// </summary>
    internal void UpdateError(string message)
    {
        ErrorMessage = message;
    }

    // --------------------------------------------------------------------
    // Input Management
    // --------------------------------------------------------------------

    public void EnqueueInput(IActionInput input)
    {
        _inputQueue.Enqueue(input);
        _history.Add(input);
    }

    public bool HasInput<T>() where T : IActionInput
    {
        return _inputQueue.Any(i => i is T);
    }

    public T? ConsumeInput<T>() where T : IActionInput
    {
        int count = _inputQueue.Count;
        for (int i = 0; i < count; i++)
        {
            var item = _inputQueue.Dequeue();
            if (item is T typedItem)
            {
                return typedItem;
            }
            _inputQueue.Enqueue(item);
        }
        return default;
    }

    public IEnumerable<IActionInput> GetAllInputs() => _history;

    // --------------------------------------------------------------------
    // Typed Data Storage (key-value store for node communication)
    // --------------------------------------------------------------------

    /// <summary>
    /// Store a value by string key.
    /// </summary>
    public void Set<T>(string key, T value) where T : notnull
    {
        _data[key] = value;
    }

    /// <summary>
    /// Retrieve a value by key.
    /// </summary>
    public T Get<T>(string key)
    {
        if (_data.TryGetValue(key, out var value) && value is T typed)
            return typed;
        throw new KeyNotFoundException($"Key '{key}' not found or wrong type");
    }

    /// <summary>
    /// Try to retrieve a value by key.
    /// </summary>
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

    /// <summary>
    /// Check if a key exists.
    /// </summary>
    public bool Has(string key)
    {
        return _data.ContainsKey(key);
    }

    /// <summary>
    /// Remove a value by key.
    /// </summary>
    public void Remove(string key)
    {
        _data.Remove(key);
    }
}

/// <summary>
/// Simple context for system actions (FSM workflows).
/// These complete immediately without suspension.
/// </summary>
public sealed class SystemActionContext : ActionContext
{
    public SystemActionContext()
    {
    }
}
