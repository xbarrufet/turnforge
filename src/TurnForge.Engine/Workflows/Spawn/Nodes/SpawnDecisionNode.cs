using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Decisions.Spawn;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Workflows.Spawn.Nodes;

/// <summary>
/// Final node: creates SpawnDecisions from processed descriptors.
/// Implements IProducesDecisions to emit decisions.
/// </summary>
public class SpawnDecisionNode : INode, IProducesDecisions
{
    public NodeId Id { get; } = new("Spawn.Decision");
    public INode? NextNode { get; set; } = null;

    public ValidationResult Validate(WorkflowContext context)
    {
        if (context is not SpawnWorkflowContext spawnContext)
        {
            return ValidationResult.CancelResult;
        }

        if (spawnContext.Descriptors.Count == 0)
        {
            return ValidationResult.CancelResult;
        }

        return ValidationResult.OkResult;
    }

    public IReadOnlyList<IDecision> BuildDecisions(WorkflowContext context)
    {
        if (context is not SpawnWorkflowContext spawnContext)
        {
            return Array.Empty<IDecision>();
        }

        var decisions = new List<IDecision>();
        foreach (var descriptor in spawnContext.Descriptors)
        {
            decisions.Add(new SpawnDecision<AgentDescriptor>(descriptor));
        }
        return decisions;
    }
}
