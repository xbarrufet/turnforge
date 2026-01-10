using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Overlay;

namespace TurnForge.Engine.Core;

/// <summary>
/// Game event that wraps a workflow operation (Move, Spawn, Destroy).
/// Used to notify UI of state changes from workflow execution.
/// </summary>
public sealed class ActionOperationEvent : IGameEvent
{
    public IGameStateOperation Operation { get; }
    public string EventType => Operation.GetType().Name;
    
    public ActionOperationEvent(IGameStateOperation operation)
    {
        Operation = operation;
    }
    
    public override string ToString() => $"ActionEvent: {EventType} on {Operation.EntityId}";
}
