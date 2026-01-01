using System;
using TurnForge.Engine.Core.Workflow.Builders;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow.Nodes;

/// <summary>
/// Base class for nodes that require external interaction/input.
/// Implements the suspend/resume loop pattern.
/// </summary>
public abstract class InteractionNode<TContext> : ILinkableNode 
    where TContext : WorkflowContext
{
    public NodeId Id { get; }
    public INode? NextNode { get; set; }

    protected InteractionNode(string id) => Id = new NodeId(id);
    
    public void SetNextNode(INode? next) => NextNode = next;

    public WorkflowStepResult Execute(WorkflowContext baseContext)
    {
        // Safety cast
        if (baseContext is not TContext context)
        {
            return WorkflowStepResult.Fail($"Invalid context type. Expected {typeof(TContext).Name}, got {baseContext.GetType().Name}");
        }

        // 1. Hook to process new inputs received since last pause
        ProcessNewInputs(context);

        // 2. Check if we are done
        if (IsReadyToComplete(context))
        {
            OnComplete(context);
            return WorkflowStepResult.Success();
        }

        // 3. If not, generate options and suspend
        var (reason, allowedInputs) = GetRequiredInteractions(context);
        return WorkflowStepResult.Suspend(reason, allowedInputs);
    }

    /// <summary>
    /// Consume inputs from queue and modify local state.
    /// Ex: "Ah, you sent 'ConfirmDefense', so I set 'DefenseConfirmed = true'"
    /// </summary>
    protected abstract void ProcessNewInputs(TContext context);

    /// <summary>
    /// Returns true if the node has fulfilled its purpose and can proceed to NextNode.
    /// </summary>
    protected abstract bool IsReadyToComplete(TContext context);

    /// <summary>
    /// Defines what we are waiting for.
    /// </summary>
    protected abstract (string Reason, Type[] AllowedInputs) GetRequiredInteractions(TContext context);

    /// <summary>
    /// (Optional) Final logic just before exiting, like committing temp values to main context.
    /// </summary>
    protected virtual void OnComplete(TContext context) { }
}
