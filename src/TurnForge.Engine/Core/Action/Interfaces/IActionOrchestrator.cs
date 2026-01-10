using System;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

/// <summary>
/// Orchestrates the execution of Actions.
/// </summary>
public interface IActionOrchestrator
{
    /// <summary>
    /// Start executing a new action.
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="gameStateView">Current game state view for node execution</param>
    /// <returns>Final or intermediate status of the action</returns>
    ActionStatus StartAction(IAction action, GameStateView gameStateView);
    
    /// <summary>
    /// Submit external input to a suspended action.
    /// </summary>
    void SubmitInput(Guid actionId, IActionInput input);
    
    /// <summary>
    /// Submit external input by string ID.
    /// </summary>
    void SubmitInput(string actionId, IActionInput input);
}
