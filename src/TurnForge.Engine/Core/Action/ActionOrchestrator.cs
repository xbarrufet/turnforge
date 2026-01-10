using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action;

/// <summary>
/// Core engine component responsible for executing Actions.
/// Manages execution, suspension, resumption and nested actions.
/// 
/// Design: Works with non-generic IAction/INode interfaces.
/// Type-safety for context is handled internally by each Action implementation.
/// </summary>
public sealed class ActionOrchestrator : IActionOrchestrator
{
    /// <summary>
    /// Tracks an active action's execution state.
    /// </summary>
    private sealed class ActionSession
    {
        public IAction Action { get; }
        public INode? CurrentNode { get; set; }
        public GameStateView GameStateView { get; }

        public ActionSession(IAction action, GameStateView gameStateView)
        {
            Action = action;
            CurrentNode = action.StartNode;
            GameStateView = gameStateView;
        }

        public bool IsComplete => CurrentNode == null;

        public void Advance()
        {
            CurrentNode = CurrentNode?.NextNode;
        }
    }

    // Active actions indexed by their ID
    private readonly Dictionary<string, ActionSession> _activeActions = new();
    private const int MAX_STEPS_SAFETY_LIMIT = 1000;
    private readonly Microsoft.Extensions.Logging.ILogger<ActionOrchestrator>? _logger;

    public ActionOrchestrator(Microsoft.Extensions.Logging.ILogger<ActionOrchestrator>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Start executing an action.
    /// </summary>
    public ActionStatus StartAction(IAction action, GameStateView gameStateView)
    {
        // 1. Assign unique Execution ID to this run
        action.Context.SetExecutionId(ActionExecutionId.New());
        
        var session = new ActionSession(action, gameStateView);
        var actionKey = action.Id.Value;
        
        _activeActions[actionKey] = session;
        action.Context.UpdateStatus(ActionStatus.Running);
        
        return ExecuteAction(actionKey);
    }

    /// <summary>
    /// Continue executing an action by its ID.
    /// </summary>
    private ActionStatus ExecuteAction(string actionId)
    {
        if (!_activeActions.TryGetValue(actionId, out var session))
        {
            throw new ArgumentException($"Action {actionId} not found");
        }

        int steps = 0;
        while (!session.IsComplete)
        {
            if (++steps > MAX_STEPS_SAFETY_LIMIT)
            {
                throw new InvalidOperationException($"Action {actionId} exceeded step limit ({MAX_STEPS_SAFETY_LIMIT}).");
            }

            // Execute current node
            ActionStepResult result;
            try 
            {
                result = session.CurrentNode!.Execute(session.Action.Context, session.GameStateView);
            }
            catch (Exception ex)
            {
                session.Action.Context.UpdateStatus(ActionStatus.Failed);
                session.Action.Context.UpdateError(ex.Message);
                _activeActions.Remove(actionId);
                _logger?.LogError(ex, "Action {ActionId} failed with exception", actionId);
                return ActionStatus.Failed;
            }

            switch (result.Status)
            {
                case ActionStatus.Completed:
                    session.Advance();
                    if (session.IsComplete)
                    {
                        session.Action.Context.UpdateStatus(ActionStatus.Completed);
                        _activeActions.Remove(actionId);
                        return ActionStatus.Completed;
                    }
                    break;

                case ActionStatus.Suspended:
                    session.Action.Context.UpdateStatus(ActionStatus.Suspended);
                    return ActionStatus.Suspended;

                case ActionStatus.Failed:
                    session.Action.Context.UpdateStatus(ActionStatus.Failed);
                    session.Action.Context.UpdateError(result.ErrorMessage ?? "Action step failed.");
                    _activeActions.Remove(actionId);
                    return ActionStatus.Failed;
            }
        }

        // All nodes executed
        session.Action.Context.UpdateStatus(ActionStatus.Completed);
        return ActionStatus.Completed;
    }

    /// <summary>
    /// Submit input to a suspended action by string ID.
    /// </summary>
    public void SubmitInput(string actionId, IActionInput input)
    {
        if (!_activeActions.TryGetValue(actionId, out var session))
        {
            throw new ArgumentException($"Action {actionId} not found or not active.");
        }
        
        // Inject input into context
        session.Action.Context.EnqueueInput(input);
        
        // Resume execution
        session.Action.Context.UpdateStatus(ActionStatus.Running);
        ExecuteAction(actionId);
    }

    /// <summary>
    /// Submit input by GUID.
    /// </summary>
    public void SubmitInput(Guid actionId, IActionInput input)
    {
        SubmitInput(actionId.ToString(), input);
    }
}
