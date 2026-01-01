using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Interfaces;

public interface INode
{
    NodeId Id { get; }
    INode? NextNode { get; set; }
    
    ActionStepResult Execute(ActionContext context);
}