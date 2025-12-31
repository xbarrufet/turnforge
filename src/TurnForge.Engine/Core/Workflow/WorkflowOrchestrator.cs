using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow;

/// <summary>
/// Core engine component responsible for executing workflows.
/// Manages execution, suspension, resumption and nested workflows.
/// </summary>
public sealed class WorkflowOrchestrator : IWorkflowOrchestrator
{
    private class WorkflowSession
    {
        public IWorkflow Workflow { get; }
        public WorkflowContext Context { get; }
        public INode? CurrentNode { get; set; }

        public WorkflowSession(IWorkflow workflow, WorkflowContext context)
        {
            Workflow = workflow;
            Context = context;
            CurrentNode = workflow.StartNode;
        }

        public void Advance()
        {
            CurrentNode = CurrentNode?.NextNode;
        }
    }

    // Use string keys to support both GUID and named workflows
    private readonly Dictionary<string, WorkflowSession> _activeWorkflows = new();
    private const int MAX_STEPS_SAFETY_LIMIT = 1000;

    /// <summary>
    /// Start a new workflow.
    /// </summary>
    public void StartWorkflow(IWorkflow workflow, WorkflowContext context)
    {
        var session = new WorkflowSession(workflow, context);
        var workflowKey = workflow.Id.Value;
        _activeWorkflows[workflowKey] = session;
        ExecuteWorkflow(workflowKey);
    }

    /// <summary>
    /// Execute workflow by its string ID.
    /// </summary>
    public WorkflowStatus ExecuteWorkflow(string workflowId)
    {
        if (!_activeWorkflows.TryGetValue(workflowId, out var session))
        {
            throw new ArgumentException($"Workflow {workflowId} not found");
        }

        int steps = 0;
        while (session.CurrentNode != null)
        {
            if (++steps > MAX_STEPS_SAFETY_LIMIT)
            {
                throw new InvalidOperationException($"Workflow {workflowId} exceeded step limit.");
            }

            // Execute the current node
            WorkflowStepResult result;
            try 
            {
                result = session.CurrentNode.Execute(session.Context);
            }
            catch (Exception)
            {
                session.Context.Status = WorkflowStatus.Failed;
                _activeWorkflows.Remove(workflowId);
                return WorkflowStatus.Failed;
            }

            switch (result.Status)
            {
                case WorkflowStatus.Completed:
                    session.Advance();
                    if (session.CurrentNode == null)
                    {
                        // Workflow completed - commit overlay to get new state
                        var newState = session.Context.Overlay.Commit(session.Context.State);
                        session.Context.UpdateState(newState);
                        
                        session.Context.Status = WorkflowStatus.Completed;
                        _activeWorkflows.Remove(workflowId);
                        return WorkflowStatus.Completed;
                    }
                    break;

                case WorkflowStatus.Suspended:
                    // Don't commit - overlay will be reused when workflow resumes
                    session.Context.Status = WorkflowStatus.Suspended;
                    return WorkflowStatus.Suspended;

                case WorkflowStatus.Failed:
                   // Don't commit - discard overlay changes
                   session.Context.Status = WorkflowStatus.Failed;
                   _activeWorkflows.Remove(workflowId);
                   return WorkflowStatus.Failed;
            }
        }

        // All nodes executed - commit and complete
        var finalState = session.Context.Overlay.Commit(session.Context.State);
        session.Context.UpdateState(finalState);
        
        session.Context.Status = WorkflowStatus.Completed;
        _activeWorkflows.Remove(workflowId);
        return WorkflowStatus.Completed;
    }

    /// <summary>
    /// Execute workflow by GUID (for backward compatibility).
    /// </summary>
    public WorkflowStatus ExecuteWorkflow(Guid workflowId)
    {
        return ExecuteWorkflow(workflowId.ToString());
    }

    /// <summary>
    /// Submit input to workflow by string ID.
    /// </summary>
    public void SubmitInput(string workflowId, IWorkflowInput input)
    {
        if (!_activeWorkflows.TryGetValue(workflowId, out var session))
        {
             throw new ArgumentException($"Workflow {workflowId} not found or not active.");
        }
        
        // 1. Inject input into context
        session.Context.EnqueueInput(input);
        
        // 2. Resume execution (re-run the current node loop)
        session.Context.Status = WorkflowStatus.Running;
        ExecuteWorkflow(workflowId);
    }

    /// <summary>
    /// Submit input to workflow by GUID (for backward compatibility).
    /// </summary>
    public void SubmitInput(Guid workflowId, IWorkflowInput input)
    {
        SubmitInput(workflowId.ToString(), input);
    }
}
