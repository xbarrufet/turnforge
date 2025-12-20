namespace TurnForge.Engine.Core.Fsm.Interfaces;

public abstract class BranchNode : FsmNode
{
    public FsmNode? FirstChild { get; internal set; }
    public List<FsmNode> Children { get; } = new();
}