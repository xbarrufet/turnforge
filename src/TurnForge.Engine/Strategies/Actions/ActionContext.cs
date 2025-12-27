using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Board;

namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Context providing access to game state, board, and session data for action strategies.
/// </summary>
/// <remarks>
/// Extended for Interactive Pipeline System:
/// - AgentId: The agent executing the action
/// - SessionId: Unique ID for this action session (for resume after suspend)
/// - Variables: Transient data stored between pipeline nodes
/// - CurrentNodeId: For resuming multi-step pipelines
/// - History: Audit trail of executed nodes
/// </remarks>
public sealed class ActionContext : IActionContext
{
    // === Core Data ===
    
    public GameState State { get; }
    public GameBoard Board { get; }
    
    // === Agent Context ===
    
    /// <summary>
    /// ID of the agent executing this action.
    /// </summary>
    public string AgentId { get; set; } = string.Empty;
    
    // === Pipeline Session ===
    
    /// <summary>
    /// Unique session ID for this action execution.
    /// Used to correlate InteractionRequest/Response.
    /// </summary>
    public Guid SessionId { get; } = Guid.NewGuid();
    
    /// <summary>
    /// Current node ID for pipeline resume (null for new actions).
    /// </summary>
    public string? CurrentNodeId { get; set; }
    
    /// <summary>
    /// Transient variables stored between pipeline nodes.
    /// Not persisted to GameState - only lives during action execution.
    /// </summary>
    public Dictionary<string, object> Variables { get; } = new();
    
    /// <summary>
    /// Audit trail of executed node IDs.
    /// </summary>
    public List<string> History { get; } = new();
    
    // === Constructors ===
    
    public ActionContext(GameState state, GameBoard board)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
        Board = board ?? throw new ArgumentNullException(nameof(board));
    }
    
    public ActionContext(GameState state, GameBoard board, string agentId) : this(state, board)
    {
        AgentId = agentId;
    }
    
    // === Variable Helpers ===
    
    /// <summary>
    /// Get a variable with optional default value.
    /// </summary>
    public T GetVariable<T>(string key, T defaultValue = default!)
    {
        if (Variables.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;
        return defaultValue;
    }
    
    /// <summary>
    /// Set a variable value.
    /// </summary>
    public void SetVariable<T>(string key, T value)
    {
        Variables[key] = value!;
    }
    
    /// <summary>
    /// Check if a variable exists.
    /// </summary>
    public bool HasVariable(string key) => Variables.ContainsKey(key);
}
