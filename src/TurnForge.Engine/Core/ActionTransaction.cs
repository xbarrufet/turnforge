using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;

/// <summary>
/// Result of a workflow execution request.
/// Contains workflow status, events generated, and the final state.
/// </summary>
public sealed class ActionTransaction
{
    public ActionId ActionId { get; }
    public ActionStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public IReadOnlyList<IGameEvent> Events { get; set; } = Array.Empty<IGameEvent>();
    public bool IsGameOver { get; set; }
    
    public ActionTransaction(ActionId workflowId)
    {
        ActionId = workflowId;
        Status = ActionStatus.NotStarted;
    }
    
    public static ActionTransaction Success(ActionId id, IReadOnlyList<IGameEvent>? events = null)
        => new(id) { Status = ActionStatus.Completed, Events = events ?? Array.Empty<IGameEvent>() };
    
    public static ActionTransaction Fail(ActionId id, string error)
        => new(id) { Status = ActionStatus.Failed, ErrorMessage = error };
        
    public static ActionTransaction Suspended(ActionId id)
        => new(id) { Status = ActionStatus.Suspended };
}
