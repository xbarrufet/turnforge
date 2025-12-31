using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow.Interfaces;

public interface INode
{
    NodeId Id { get; }
    INode? NextNode { get; set; }
    
    WorkflowStepResult Execute(WorkflowContext context);
}