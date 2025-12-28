using System;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow.Interfaces;

public interface IWorkflowOrchestrator
{
    WorkflowExecutionResult Execute(
        IWorkflow workflow,
        WorkflowContext context);

    WorkflowExecutionResult Resume(
        IWorkflow workflow,
        WorkflowContext context,
        IInputActionResult input,
        Func<WorkflowId, IWorkflow>? workflowResolver = null);
}
