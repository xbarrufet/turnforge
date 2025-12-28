using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Decisions.Entity.Interfaces;

namespace TurnForge.Engine.Core.Workflow;

/// <summary>
/// Core engine component responsible for executing workflows.
/// Manages execution, suspension, resumption and nested workflows.
/// </summary>
public sealed class WorkflowOrchestrator : IWorkflowOrchestrator
{
    private const int MAX_STEPS_SAFETY_LIMIT = 1000;

    public WorkflowExecutionResult Execute(
        IWorkflow workflow,
        WorkflowContext context)
    {
        ArgumentNullException.ThrowIfNull(workflow);
        ArgumentNullException.ThrowIfNull(context);

        context.Status = WorkflowStatus.Running;
        context.PushFrame(workflow.Id, workflow.StartNode.Id);

        var result = RunLoop(workflow.StartNode, context, workflow);

        if (result.Status == WorkflowStatus.Completed)
        {
            context.PopFrame();
        }

        return result;
    }

    public WorkflowExecutionResult Resume(
        IWorkflow workflow,
        WorkflowContext context,
        IInputActionResult input,
        Func<WorkflowId, IWorkflow>? workflowResolver = null)
    {
        ArgumentNullException.ThrowIfNull(workflow);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(input);

        if (context.Status != WorkflowStatus.Suspended)
            throw new InvalidOperationException($"Cannot resume workflow in state {context.Status}");

        if (context.CurrentNodeId is null)
            throw new InvalidOperationException("Cannot resume without CurrentNodeId.");

        bool resumedFromChild = false;

        if (context.NavigationStack.Count > 0)
        {
            var topFrame = context.PeekFrame();
            if (topFrame.WorkflowId != workflow.Id)
            {
                if (workflowResolver is null)
                    throw new InvalidOperationException("Nested workflow resolver required.");

                var childWorkflow = workflowResolver(topFrame.WorkflowId);
                var childResult = Resume(childWorkflow, context, input, workflowResolver);

                if (childResult.Status != WorkflowStatus.Completed)
                    return childResult;

                resumedFromChild = true;
            }
        }

        context.Status = WorkflowStatus.Running;
        var node = workflow.GetNode(context.CurrentNodeId.Value);

        if (!resumedFromChild)
        {
            if (node is not IAcceptsInput)
                throw new InvalidOperationException($"Node {node.Id} does not accept input.");

            ProcessInput(node, context, input);
        }

        var result = RunLoop(node.NextNode, context, workflow);

        if (result.Status == WorkflowStatus.Completed)
        {
            context.PopFrame();
        }

        return result;
    }

    private WorkflowExecutionResult RunLoop(
        INode? startNode,
        WorkflowContext context,
        IWorkflow workflow)
    {
        int steps = 0;
        INode? currentNode = startNode;

        try
        {
            while (currentNode != null)
            {
                if (++steps > MAX_STEPS_SAFETY_LIMIT)
                    throw new InvalidOperationException($"Workflow {workflow.Id} exceeded step limit.");

                context.UpdateCurrentNode(currentNode.Id);

                // 1. Validation
                var validation = currentNode.Validate(context);
                if (validation is ValidationResult.Cancel)
                {
                    context.Status = WorkflowStatus.Cancelled;
                    return WorkflowExecutionResult.Cancelled();
                }

                if (validation is ValidationResult.Suspend)
                {
                    context.Status = WorkflowStatus.Suspended;
                    return WorkflowExecutionResult.Suspended();
                }

                if (validation is ValidationResult.Redirect)
                    throw new NotImplementedException("Redirect not supported yet.");

                bool nodeAlreadyProcessed = false;

                // 2. Reactions
                if (currentNode is IAcceptsReactions reactor)
                {
                    foreach (var reaction in reactor.AllowedReactions)
                    {
                        if (!reaction.CanReact(context))
                            continue;

                        var reactionResult = reaction.React(context, null);

                        // a) Reaction requires player decision (or any other input requirement)
                        if (reactionResult.RequiresInput)
                        {
                            context.Status = WorkflowStatus.Suspended;
                            return WorkflowExecutionResult.Suspended();
                        }

                        // b) Reaction auto-provides input
                        if (reactionResult.ModifiedInput != null)
                        {
                            ProcessInput(currentNode, context, reactionResult.ModifiedInput);
                            nodeAlreadyProcessed = true;
                        }

                        // c) Nested workflow execution
                        if (reactionResult.NestedWorkflow != null &&
                            reactionResult.ExecuteNestedWorkflow)
                        {
                            context.PushFrame(
                                reactionResult.NestedWorkflow.Id,
                                reactionResult.NestedWorkflow.StartNode.Id,
                                reaction.Id);

                            var nestedResult = RunLoop(
                                reactionResult.NestedWorkflow.StartNode,
                                context,
                                reactionResult.NestedWorkflow);

                            if (nestedResult.Status != WorkflowStatus.Completed)
                                return nestedResult;

                            context.PopFrame();
                        }
                    }
                }

                // 3. Input detection
                if (currentNode is IAcceptsInput && !nodeAlreadyProcessed)
                {
                    context.Status = WorkflowStatus.Suspended;
                    return WorkflowExecutionResult.Suspended();
                }

                // 4. Collect Decisions (Phase 5)
                if (currentNode is IProducesDecisions producer)
                {
                    var decisions = producer.BuildDecisions(context);
                    foreach (var d in decisions)
                    {
                        context.RecordDecision(d);
                    }
                }

                // 5. Event Processing (Phase 5 Refactor: Intermediate Events)
                // Process pending events immediately after the node executes (and produces decisions/events)
                var eventResult = ProcessPendingEvents(workflow, context);
                if (eventResult != null) return eventResult;

                // 6. Advance
                if (currentNode.NextNode != null)
                {
                    context.RecordTransition(currentNode.Id, currentNode.NextNode.Id);
                    currentNode = currentNode.NextNode;
                }
                else
                {
                    // End of workflow
                    break;
                }
            }

            context.Status = WorkflowStatus.Completed;
            return WorkflowExecutionResult.Completed();
        }
        catch
        {
            context.Status = WorkflowStatus.Cancelled;
            throw;
        }
    }

    private WorkflowExecutionResult? ProcessPendingEvents(IWorkflow workflow, WorkflowContext context)
    {
        while (context.HasPendingEvents)
        {
            bool reactionTriggered = false;
            foreach (var reaction in workflow.GlobalReactions)
            {
                if (reaction.CanReact(context))
                {
                    reactionTriggered = true;
                    var result = reaction.React(context, null);

                    if (result.RequiresInput)
                    {
                        context.Status = WorkflowStatus.Suspended;
                        return WorkflowExecutionResult.Suspended();
                    }

                    if (result.NestedWorkflow != null && result.ExecuteNestedWorkflow)
                    {
                        context.PushFrame(
                            result.NestedWorkflow.Id,
                            result.NestedWorkflow.StartNode.Id,
                            reaction.Id);

                        var nestedResult = RunLoop(
                            result.NestedWorkflow.StartNode,
                            context,
                            result.NestedWorkflow);

                        if (nestedResult.Status != WorkflowStatus.Completed)
                            return nestedResult;

                        context.PopFrame();
                    }
                }
            }

            // Safety: If events exist but no reaction matched, we must discard them to avoid infinite loop
            if (!reactionTriggered && context.HasPendingEvents)
            {
                context.ClearEvents();
            }
        }
        return null;
    }

    private void ProcessInput(
        INode node,
        WorkflowContext context,
        IInputActionResult input)
    {
        var method = node.GetType().GetMethod("MoveForward");
        if (method == null)
            throw new InvalidOperationException(
                $"Node {node.GetType().Name} accepts input but has no MoveForward.");

        try
        {
            method.Invoke(node, new object[] { context, input });
        }
        catch (System.Reflection.TargetInvocationException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }
}
