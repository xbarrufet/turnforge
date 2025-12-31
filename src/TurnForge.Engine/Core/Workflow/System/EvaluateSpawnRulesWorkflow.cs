using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Core.Workflow.Nodes;
using TurnForge.Engine.Entities.Spawn;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Core.Fsm;

namespace TurnForge.Engine.Core.Workflow.System;

/// <summary>
/// System workflow to evaluate spawn rules and generate entities automatically.
/// Uses SpawnOrchestrator to check rules against current game state.
/// </summary>
public class EvaluateSpawnRulesWorkflow : IWorkflow
{
    private readonly SpawnOrchestrator _spawnOrchestrator;
    private readonly NodeId _startNodeId = new("Start");
    
    public WorkflowId Id => new("EvaluateSpawnRules");
    public INode StartNode { get; }
    public IReadOnlyCollection<IReaction> GlobalReactions => Array.Empty<IReaction>();
    
    public EvaluateSpawnRulesWorkflow(SpawnOrchestrator spawnOrchestrator)
    {
        _spawnOrchestrator = spawnOrchestrator;
        
        StartNode = new SpawnEvaluationNode(_startNodeId, _spawnOrchestrator);
    }
    
    public INode GetNode(NodeId nodeId)
    {
        if (nodeId == _startNodeId) return StartNode;
        throw new KeyNotFoundException($"Node {nodeId.Value} not found in EvaluateSpawnRulesWorkflow");
    }
}

/// <summary>
/// Node that performs the actual spawn evaluation.
/// </summary>
internal class SpawnEvaluationNode : INode
{
    private readonly SpawnOrchestrator _spawnOrchestrator;
    
    public NodeId Id { get; }
    public INode? NextNode { get; set; }
    
    public SpawnEvaluationNode(NodeId id, SpawnOrchestrator spawnOrchestrator)
    {
        Id = id;
        _spawnOrchestrator = spawnOrchestrator;
    }
    
    public WorkflowStepResult Execute(WorkflowContext context)
    {
        // Get state from context
        var state = context.State;
        
        // Use context's overlay (shared across all workflow nodes)
        var view = new Entities.GameStateView(state, context.Overlay);
        
        // Evaluate rules and record operations to the context's overlay
        // The orchestrator will commit this overlay when the workflow completes
        _spawnOrchestrator.ExecuteSpawns(view, context.Overlay);
        
        return WorkflowStepResult.Success();
    }
}
