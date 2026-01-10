using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

/// <summary>
/// Interface for FSM nodes (for dependency injection and abstraction).
/// </summary>
public interface IFsmNode
{
    NodeId Id { get; }
    string Name { get; }
    
    bool IsActionAllowed(ActionId actionId);
    IReadOnlyList<ActionId> GetAllowedActions();
    bool IsCompleted(GameStateView state);
    BaseFsmNode? GetNextNode(GameStateView state);
}
