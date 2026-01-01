using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Builders;

/// <summary>
/// Fluent builder for creating workflows.
/// Simplifies workflow construction by chaining nodes and reactions.
/// </summary>
public class ActionBuilder
{
    private readonly string _id;
    private readonly List<INode> _nodes = new();
    private readonly List<IReaction> _reactions = new();
    
    private ActionBuilder(string id)
    {
        _id = id;
    }
    
    /// <summary>
    /// Start building a new workflow.
    /// </summary>
    public static ActionBuilder Create(string workflowId)
    {
        return new ActionBuilder(workflowId);
    }
    
    /// <summary>
    /// Add a node to the workflow chain.
    /// Nodes are linked in the order they are added.
    /// </summary>
    public ActionBuilder AddNode<TNode>() where TNode : INode, new()
    {
        _nodes.Add(new TNode());
        return this;
    }
    
    /// <summary>
    /// Add a node instance to the workflow chain.
    /// </summary>
    public ActionBuilder AddNode(INode node)
    {
        _nodes.Add(node);
        return this;
    }
    
    /// <summary>
    /// Add a global reaction to the workflow.
    /// </summary>
    public ActionBuilder WithReaction<TReaction>() where TReaction : IReaction, new()
    {
        _reactions.Add(new TReaction());
        return this;
    }
    
    /// <summary>
    /// Add a reaction instance to the workflow.
    /// </summary>
    public ActionBuilder WithReaction(IReaction reaction)
    {
        _reactions.Add(reaction);
        return this;
    }
    
    /// <summary>
    /// Build the workflow.
    /// </summary>
    public IAction Build()
    {
        if (_nodes.Count == 0)
            throw new InvalidOperationException("Action must have at least one node");
        
        // Link nodes in sequence
        for (int i = 0; i < _nodes.Count - 1; i++)
        {
            if (_nodes[i] is ILinkableNode linkable)
            {
                linkable.SetNextNode(_nodes[i + 1]);
            }
        }
        
        return new BuiltAction(
            new ActionId(_id),
            _nodes[0],
            _nodes.ToDictionary(n => n.Id, n => n),
            _reactions.AsReadOnly()
        );
    }
}

/// <summary>
/// Interface for nodes that can have their NextNode set by the builder.
/// </summary>
public interface ILinkableNode : INode
{
    void SetNextNode(INode? next);
}

/// <summary>
/// Base class for nodes that support builder linking.
/// </summary>
public abstract class LinkableNode : ILinkableNode
{
    public abstract NodeId Id { get; }
    public INode? NextNode { get; set; }
    
    public void SetNextNode(INode? next) => NextNode = next;
    
    public abstract ActionStepResult Execute(ActionContext context);
}

/// <summary>
/// Action created by the builder.
/// </summary>
internal sealed class BuiltAction : IAction
{
    public ActionId Id { get; }
    public INode StartNode { get; }
    public IReadOnlyCollection<IReaction> GlobalReactions { get; }
    
    private readonly Dictionary<NodeId, INode> _nodes;
    
    public BuiltAction(
        ActionId id,
        INode startNode,
        Dictionary<NodeId, INode> nodes,
        IReadOnlyCollection<IReaction> reactions)
    {
        Id = id;
        StartNode = startNode;
        _nodes = nodes;
        GlobalReactions = reactions;
    }
    
    public INode GetNode(NodeId nodeId) => _nodes[nodeId];
}
