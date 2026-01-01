using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action;

public class BuildGameNode : INode
{
    public NodeId Id { get; } = new("StartGame.BuildGame");
    public INode? NextNode { get; set; }

    public ActionStepResult Execute(ActionContext context)
    {
        // TODO: Build game entities
        return ActionStepResult.Success();
    }
}