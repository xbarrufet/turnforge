// ============================================================================
// TurnForge.Engine – ActionBuilder (Non-Generic)
// ============================================================================
//
// Fluent builder for creating Actions.
// The context type is specified once and actions cast internally.
// ============================================================================

using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Action.Builders;

/// <summary>
/// Fluent builder for creating Action workflows.
/// </summary>
public class ActionBuilder
{
    private readonly string _id;
    private readonly List<INode> _nodes = new();
    private readonly List<IReaction> _reactions = new();
    private Func<ActionContext>? _contextFactory;

    private ActionBuilder(string id)
    {
        _id = id;
    }

    /// <summary>
    /// Create a new action builder with the specified ID.
    /// </summary>
    public static ActionBuilder Create(string actionId)
    {
        return new ActionBuilder(actionId);
    }

    /// <summary>
    /// Set the factory for creating the typed context.
    /// </summary>
    public ActionBuilder WithContext<TContext>(Func<TContext> factory) where TContext : ActionContext
    {
        _contextFactory = () => factory();
        return this;
    }

    /// <summary>
    /// Add a node by creating instance of type.
    /// </summary>
    public ActionBuilder AddNode<TNode>() where TNode : INode, new()
    {
        _nodes.Add(new TNode());
        return this;
    }

    /// <summary>
    /// Add a pre-constructed node.
    /// </summary>
    public ActionBuilder AddNode(INode node)
    {
        _nodes.Add(node);
        return this;
    }

    /// <summary>
    /// Add a reaction by creating instance of type.
    /// </summary>
    public ActionBuilder WithReaction<TReaction>() where TReaction : IReaction, new()
    {
        _reactions.Add(new TReaction());
        return this;
    }

    /// <summary>
    /// Add a pre-constructed reaction.
    /// </summary>
    public ActionBuilder WithReaction(IReaction reaction)
    {
        _reactions.Add(reaction);
        return this;
    }

    /// <summary>
    /// Build the action, linking nodes together.
    /// </summary>
    public IAction Build()
    {
        if (_nodes.Count == 0)
            throw new InvalidOperationException("Action must have at least one node");

        if (_contextFactory == null)
            throw new InvalidOperationException("Must specify WithContext<TContext>() before building");

        // Link nodes together
        for (int i = 0; i < _nodes.Count - 1; i++)
        {
            if (_nodes[i] is ILinkableNode linkable)
            {
                linkable.SetNextNode(_nodes[i + 1]);
            }
            else
            {
                _nodes[i].NextNode = _nodes[i + 1];
            }
        }

        var context = _contextFactory();
        
        return new BuiltAction(
            new ActionId(_id),
            _nodes[0],
            _nodes.ToDictionary(n => n.Id, n => n),
            _reactions.AsReadOnly(),
            context
        );
    }
}

/// <summary>
/// Base class for nodes that can be linked together.
/// Subclasses should cast the context to their expected type.
/// </summary>
public abstract class LinkableNode : ILinkableNode
{
    public abstract NodeId Id { get; }
    public INode? NextNode { get; set; }

    public void SetNextNode(INode? next) => NextNode = next;

    public abstract ActionStepResult Execute(ActionContext context, GameStateView state);
    
    /// <summary>
    /// Helper to safely cast context to expected type.
    /// </summary>
    protected TContext GetTypedContext<TContext>(ActionContext context) where TContext : ActionContext
    {
        if (context is TContext typed)
            return typed;
        
        throw new InvalidOperationException(
            $"Expected context of type {typeof(TContext).Name} but got {context.GetType().Name}");
    }
}

/// <summary>
/// Internal implementation of IAction created by ActionBuilder.
/// </summary>
internal sealed class BuiltAction : IAction
{
    public ActionId Id { get; }
    public INode StartNode { get; }
    public ActionContext Context { get; }
    public IReadOnlyCollection<IReaction> GlobalReactions { get; }

    private readonly Dictionary<NodeId, INode> _nodes;

    public BuiltAction(
        ActionId id,
        INode startNode,
        Dictionary<NodeId, INode> nodes,
        IReadOnlyCollection<IReaction> reactions,
        ActionContext context)
    {
        Id = id;
        StartNode = startNode;
        _nodes = nodes;
        GlobalReactions = reactions;
        Context = context;
    }

    public INode GetNode(NodeId nodeId) => _nodes[nodeId];
}
