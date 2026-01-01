using System;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow.Interfaces;

public interface IWorkflowOrchestrator
{
    void StartWorkflow(IWorkflow workflow, WorkflowContext context);
    void SubmitInput(Guid workflowId, IWorkflowInput input);
}
