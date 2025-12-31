using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

/// <summary>
/// FSM node contract for the unified FSM 2.0 architecture.
/// Nodes are generic - their behavior is determined by configuration.
/// </summary>
public interface IFsmNode
{
    NodeId Id { get; }
    string Name { get; }
    
    /// <summary>
    /// Determine the next node based on current game state.
    /// Returns null if this is a terminal node (game over).
    /// </summary>
    IFsmNode? GetNextNode(GameState state);
    
    /// <summary>
    /// Check if a command type is allowed in this node.
    /// </summary>
    bool IsCommandAllowed(Type commandType);
    
    /// <summary>
    /// List of command types allowed in this node.
    /// Empty = passthrough node (no user interaction).
    /// </summary>
    IReadOnlyList<Type> AllowedCommands { get; }
    
    /// <summary>
    /// Check if this node is completed and should transition.
    /// Pure function based on GameState.
    /// </summary>
    bool IsCompleted(GameState state);
    
    /// <summary>
    /// Resolvers to execute when entering this node (in order).
    /// [Deprecated] Use OnEntryWorkflows instead for new code.
    /// </summary>
    IReadOnlyList<INodeResolver> Resolvers { get; }
    
    /// <summary>
    /// System workflows to execute when entering this node (in order).
    /// These workflows run automatically without user input.
    /// </summary>
    IReadOnlyList<IWorkflow> OnEntryWorkflows { get; }
}
