using TurnForge.Engine.Strategies.Actions;
using TurnForge.Engine.Strategies.Interactions;
using TurnForge.Engine.Decisions.Actions;

namespace TurnForge.Engine.Strategies.Pipelines;

/// <summary>
/// A single step in an interactive pipeline.
/// </summary>
/// <remarks>
/// Nodes can:
/// - Execute logic and continue to next node
/// - Suspend execution for user input
/// - Commit decisions (end of pipeline)
/// 
/// Nodes are STATELESS. Use ActionContext.Variables for data passing.
/// </remarks>
public interface IInteractionNode
{
    /// <summary>
    /// Unique identifier for this node in the pipeline.
    /// </summary>
    string NodeId { get; }
    
    /// <summary>
    /// Execute this node's logic.
    /// </summary>
    /// <param name="context">Action context with session data and variables</param>
    /// <returns>Result determining next action (continue, suspend, commit)</returns>
    NodeResult Execute(ActionContext context);
}

/// <summary>
/// Result of executing an interaction node.
/// </summary>
public record NodeResult
{
    /// <summary>
    /// If true, pause execution and request user input.
    /// Interaction property should be populated.
    /// </summary>
    public bool Suspend { get; init; }
    
    /// <summary>
    /// Interaction request for UI (only if Suspend=true).
    /// </summary>
    public InteractionRequest? Request { get; init; }
    
    /// <summary>
    /// ID of next node to execute (null = end of pipeline).
    /// Ignored if Commit=true.
    /// </summary>
    public string? NextNodeId { get; init; }
    
    /// <summary>
    /// If true, this node ends the pipeline with decisions.
    /// Decisions property should be populated.
    /// </summary>
    public bool Commit { get; init; }
    
    /// <summary>
    /// Decisions to apply (only if Commit=true).
    /// </summary>
    public IReadOnlyList<ActionDecision>? Decisions { get; init; }
    
    // === Factory Methods ===
    
    /// <summary>
    /// Continue to next node.
    /// </summary>
    public static NodeResult Continue(string nextNodeId)
        => new() { NextNodeId = nextNodeId };
    
    /// <summary>
    /// Suspend and request user input.
    /// </summary>
    public static NodeResult SuspendFor(InteractionRequest request)
        => new() { Suspend = true, Request = request };
    
    /// <summary>
    /// Commit decisions and end pipeline.
    /// </summary>
    public static NodeResult CommitWith(IEnumerable<ActionDecision> decisions)
        => new() { Commit = true, Decisions = decisions.ToList() };
    
    /// <summary>
    /// End pipeline with no decisions (e.g., cancelled).
    /// </summary>
    public static NodeResult End()
        => new() { NextNodeId = null };
}
