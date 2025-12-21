namespace TurnForge.Engine.Core.Fsm.Interfaces;

public abstract class BranchNode : FsmNode
{
    public FsmNode? FirstChild { get; internal set; }
    public List<FsmNode> Children { get; } = new();

    public override bool IsCommandAllowed(Type commandType)
    {
        // in branches nodes, commands are not allowed
        return false;
    }

    public override IReadOnlyList<Type> GetAllowedCommands() => Array.Empty<Type>();
}