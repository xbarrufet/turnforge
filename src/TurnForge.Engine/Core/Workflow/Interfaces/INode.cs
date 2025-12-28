using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Workflow.Interfaces;

 public interface INode
{
        NodeId Id { get; }

        ValidationResult Validate(WorkflowContext context);

        INode? NextNode { get; }
}