using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Workflows.Spawn.Nodes;

/// <summary>
/// Handles position assignment for spawning entities.
/// Base implementation passes through as-is.
/// Game-specific reactions can modify positions.
/// </summary>
public class SpawnPlacementNode : INode
{
    public NodeId Id { get; } = new("Spawn.Placement");
    public INode? NextNode { get; set; }

    public ValidationResult Validate(WorkflowContext context)
    {
        // Base implementation: positions already set from request/definition
        // Game-specific Reactions can hook here to:
        // - Find spawn points
        // - Resolve collisions
        // - Apply random offsets

        return ValidationResult.OkResult;
    }
}
