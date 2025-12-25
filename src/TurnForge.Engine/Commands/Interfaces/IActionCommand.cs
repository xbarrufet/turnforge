using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.Interfaces;

/// <summary>
/// Marker interface for player-initiated actions that may cost Action Points.
/// </summary>
/// <remarks>
/// HasCost enables early validation: if true and agent has 0 AP, 
/// command can be rejected before calling the handler.
/// </remarks>
public interface IActionCommand : ICommand
{
    /// <summary>
    /// Whether this action costs Action Points (strategy calculates exact cost).
    /// </summary>
    bool HasCost { get; }
    
    /// <summary>
    /// ID of the agent performing the action.
    /// </summary>
    string AgentId { get; }  
}