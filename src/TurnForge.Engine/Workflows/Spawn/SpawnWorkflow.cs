using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Workflows.Spawn.Nodes;

namespace TurnForge.Engine.Workflows.Spawn;

/// <summary>
/// Workflow for spawning agents.
/// Pipeline: Validation → Processing → Placement → Decision Creation
/// </summary>
public class SpawnWorkflow : IWorkflow
{
    public WorkflowId Id { get; } = new("Engine.SpawnAgents");
    public INode StartNode { get; }
    public IReadOnlyCollection<IReaction> GlobalReactions { get; }

    private readonly Dictionary<string, INode> _nodes = new();

    public SpawnWorkflow()
    {
        // Build node chain
        var validation = new SpawnValidationNode();
        var processing = new SpawnProcessingNode();
        var placement = new SpawnPlacementNode();
        var decision = new SpawnDecisionNode();

        validation.NextNode = processing;
        processing.NextNode = placement;
        placement.NextNode = decision;

        StartNode = validation;
        GlobalReactions = new List<IReaction>();

        // Register all nodes for GetNode lookup
        _nodes[validation.Id.Value] = validation;
        _nodes[processing.Id.Value] = processing;
        _nodes[placement.Id.Value] = placement;
        _nodes[decision.Id.Value] = decision;
    }

    public INode GetNode(NodeId nodeId)
    {
        return _nodes[nodeId.Value];
    }
}
