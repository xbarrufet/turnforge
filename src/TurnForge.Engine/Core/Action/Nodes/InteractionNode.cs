using System;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Nodes;

/// <summary>
/// Base class for nodes that require external interaction/input.
/// Implements the suspend/resume loop pattern.
/// 
/// Generic parameter TContext allows subclasses to work with typed contexts
/// while implementing the non-generic INode interface.
/// </summary>
public abstract class InteractionNode<TContext> : LinkableNode
    where TContext : ActionContext
{
    public override NodeId Id { get; }

    protected InteractionNode(string id) => Id = new NodeId(id);

    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        // Cast to typed context
        var typedContext = GetTypedContext<TContext>(context);

        // 1. Hook to process new inputs received since last pause
        ProcessNewInputs(typedContext, state);

        // 2. Check if we are done
        if (IsReadyToComplete(typedContext))
        {
            OnComplete(typedContext, state);
            return ActionStepResult.Success();
        }

        // 3. If not, generate options and suspend
        var (reason, allowedInputs) = GetRequiredInteractions(typedContext);
        return ActionStepResult.Suspend(reason, allowedInputs);
    }

    /// <summary>
    /// Consume inputs from queue and modify local state.
    /// Use state.RecordOperation() to record changes.
    /// </summary>
    protected abstract void ProcessNewInputs(TContext context, GameStateView state);

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
    protected virtual void OnComplete(TContext context, GameStateView state) { }
}
