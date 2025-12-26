using TurnForge.Engine.Strategies.Actions;

namespace TurnForge.Engine.Strategies.Pipelines;

/// <summary>
/// Registry to track active interaction sessions.
/// Maps session IDs to their ActionContext for resume.
/// </summary>
/// <remarks>
/// When a strategy suspends, its context is stored here.
/// When SubmitInteractionCommand arrives, context is retrieved to resume.
/// 
/// Thread-safe for concurrent access.
/// </remarks>
public class InteractionRegistry
{
    private readonly Dictionary<Guid, ActionContext> _sessions = new();
    private readonly object _lock = new();
    
    /// <summary>
    /// Register a suspended session.
    /// </summary>
    public void Register(ActionContext context)
    {
        lock (_lock)
        {
            _sessions[context.SessionId] = context;
        }
    }
    
    /// <summary>
    /// Get context for a session ID.
    /// </summary>
    public ActionContext? Get(Guid sessionId)
    {
        lock (_lock)
        {
            return _sessions.TryGetValue(sessionId, out var context) ? context : null;
        }
    }
    
    /// <summary>
    /// Remove a completed/cancelled session.
    /// </summary>
    public void Remove(Guid sessionId)
    {
        lock (_lock)
        {
            _sessions.Remove(sessionId);
        }
    }
    
    /// <summary>
    /// Check if a session exists.
    /// </summary>
    public bool HasSession(Guid sessionId)
    {
        lock (_lock)
        {
            return _sessions.ContainsKey(sessionId);
        }
    }
    
    /// <summary>
    /// Get count of active sessions.
    /// </summary>
    public int ActiveCount
    {
        get
        {
            lock (_lock)
            {
                return _sessions.Count;
            }
        }
    }
}
