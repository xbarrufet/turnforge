using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action;

/// <summary>
/// Core engine component responsible for executing workflows.
/// Manages execution, suspension, resumption and nested workflows.
/// </summary>
public sealed class ActionOrchestrator : IActionOrchestrator
{
    private class ActionSession
    {
        public IAction Action { get; }
        public ActionContext Context { get; }
        public INode? CurrentNode { get; set; }

        public ActionSession(IAction workflow, ActionContext context)
        {
            Action = workflow;
            Context = context;
            CurrentNode = workflow.StartNode;
        }

        public void Advance()
        {
            CurrentNode = CurrentNode?.NextNode;
        }
    }

    // Use string keys to support both GUID and named workflows
    private readonly Dictionary<string, ActionSession> _activeActions = new();
    private const int MAX_STEPS_SAFETY_LIMIT = 1000;
    private readonly Microsoft.Extensions.Logging.ILogger<ActionOrchestrator>? _logger;

    public ActionOrchestrator(Microsoft.Extensions.Logging.ILogger<ActionOrchestrator>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Start a new workflow.
    /// </summary>
    public void StartAction(IAction workflow, ActionContext context)
    {
        var session = new ActionSession(workflow, context);
        var workflowKey = workflow.Id.Value;
        _activeActions[workflowKey] = session;
        ExecuteAction(workflowKey);
    }

    /// <summary>
    /// Execute workflow by its string ID.
    /// </summary>
    public ActionStatus ExecuteAction(string workflowId)
    {
        if (!_activeActions.TryGetValue(workflowId, out var session))
        {
            throw new ArgumentException($"Action {workflowId} not found");
        }

        int steps = 0;
        while (session.CurrentNode != null)
        {
            if (++steps > MAX_STEPS_SAFETY_LIMIT)
            {
                throw new InvalidOperationException($"Action {workflowId} exceeded step limit.");
            }

            // Execute the current node
            ActionStepResult result;
            try 
            {
                result = session.CurrentNode.Execute(session.Context);
            }
            catch (Exception)
            {
                session.Context.Status = ActionStatus.Failed;
                _activeActions.Remove(workflowId);
                return ActionStatus.Failed;
            }

            switch (result.Status)
            {
                case ActionStatus.Completed:
                    session.Advance();
                    if (session.CurrentNode == null)
                    {
                        // Action completed - commit overlay to get new state
                        var newState = session.Context.Overlay.Commit();
                        session.Context.UpdateState(newState);
                        
                        session.Context.Status = ActionStatus.Completed;
                        _activeActions.Remove(workflowId);
                        return ActionStatus.Completed;
                    }
                    break;

                case ActionStatus.Suspended:
                    // Don't commit - overlay will be reused when workflow resumes
                    session.Context.Status = ActionStatus.Suspended;
                    return ActionStatus.Suspended;

                case ActionStatus.Failed:
                   // Don't commit - discard overlay changes
                   session.Context.Status = ActionStatus.Failed;
                   _activeActions.Remove(workflowId);
                   return ActionStatus.Failed;
            }
        }

        // All nodes executed - commit and complete
        var finalState = session.Context.Overlay.Commit();
        session.Context.UpdateState(finalState);
        
        session.Context.Status = ActionStatus.Completed;
        _activeActions.Remove(workflowId);
        return ActionStatus.Completed;
    }

    /// <summary>
    /// Execute workflow by GUID (for backward compatibility).
    /// </summary>
    public ActionStatus ExecuteAction(Guid workflowId)
    {
        return ExecuteAction(workflowId.ToString());
    }

    /// <summary>
    /// Submit input to workflow by string ID.
    /// </summary>
    public void SubmitInput(string workflowId, IActionInput input)
    {
        if (!_activeActions.TryGetValue(workflowId, out var session))
        {
             throw new ArgumentException($"Action {workflowId} not found or not active.");
        }
        
        // 1. Inject input into context
        session.Context.EnqueueInput(input);
        
        // 2. Resume execution (re-run the current node loop)
        session.Context.Status = ActionStatus.Running;
        ExecuteAction(workflowId);
    }

    /// <summary>
    /// Submit input to workflow by GUID (for backward compatibility).
    /// </summary>
    public void SubmitInput(Guid workflowId, IActionInput input)
    {
        SubmitInput(workflowId.ToString(), input);
    }
}
