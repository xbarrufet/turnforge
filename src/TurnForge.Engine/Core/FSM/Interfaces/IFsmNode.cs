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
    
    bool IsCommandAllowed(Type commandType);
    IReadOnlyList<Type> GetAllowedCommands();
    bool IsCompleted(GameState state);
    BaseFsmNode? GetNextNode(GameState state);
}
