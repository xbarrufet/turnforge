using TurnForge.Engine.Core.Action.Interfaces;

namespace TurnForge.Engine.Core.Fsm;

/// <summary>
/// Fluent builder for FSM construction (FSM 2.0).
/// </summary>
public class FsmBuilder
{
    private readonly List<BaseFsmNode> _nodes = new();
    private BaseFsmNode? _rootNode;
    
    public static FsmBuilder Create() => new();
    
    /// <summary>
    /// Set the root node.
    /// </summary>
    public FsmBuilder WithRoot(BaseFsmNode node)
    {
        _rootNode = node;
        _nodes.Add(node);
        return this;
    }
    
    /// <summary>
    /// Add a node to the graph.
    /// </summary>
    public FsmBuilder WithNode(BaseFsmNode node)
    {
        _nodes.Add(node);
        return this;
    }
    
    /// <summary>
    /// Build the FSM graph (simple version for tests).
    /// </summary>
    public FsmGraph Build()
    {
        var graph = new FsmGraph();
        
        foreach (var node in _nodes)
        {
            graph.AddNode(node);
        }
        
        if (_rootNode != null)
        {
            graph.SetRoot(_rootNode);
        }
        
        return graph;
    }
}
