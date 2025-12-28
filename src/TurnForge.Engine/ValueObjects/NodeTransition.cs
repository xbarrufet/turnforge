namespace TurnForge.Engine.ValueObjects;

/// <summary>
    /// Value object describing a transition between nodes.
    /// Used for tracing and debugging workflow execution.
    /// </summary>
    public readonly record struct NodeTransition(
        NodeId From,
        NodeId To
    );