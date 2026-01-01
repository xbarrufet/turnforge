using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Workflow;

public class BuildGameNode : INode
{
    public NodeId Id { get; } = new("StartGame.BuildGame");
    public INode? NextNode { get; set; }

    public WorkflowStepResult Execute(WorkflowContext context)
    {
        // TODO: Build game entities
        return WorkflowStepResult.Success();
    }
}