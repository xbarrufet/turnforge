using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Workflows.Spawn.Nodes;

/// <summary>
/// Validates spawn requests before processing.
/// Checks: requests exist, catalog available.
/// </summary>
public class SpawnValidationNode : INode
{
    public NodeId Id { get; } = new("Spawn.Validation");
    public INode? NextNode { get; set; }

    public ValidationResult Validate(WorkflowContext context)
    {
        if (context is not SpawnWorkflowContext spawnContext)
        {
            return ValidationResult.CancelResult;
        }

        if (spawnContext.Requests.Count == 0)
        {
            return ValidationResult.CancelResult;
        }

        if (spawnContext.Catalog == null)
        {
            return ValidationResult.CancelResult;
        }

        return ValidationResult.OkResult;
    }
}
