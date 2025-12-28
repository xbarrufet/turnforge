using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;

namespace TurnForge.Engine.Workflows.Spawn;

/// <summary>
/// Workflow context for spawn operations.
/// Carries spawn requests through validation, processing, and decision creation.
/// </summary>
public class SpawnWorkflowContext : WorkflowContext
{
    /// <summary>
    /// Original spawn requests from the command.
    /// </summary>
    public IReadOnlyList<SpawnRequest> Requests { get; }

    /// <summary>
    /// Descriptors after preprocessing (built from requests + definitions).
    /// </summary>
    public List<AgentDescriptor> Descriptors { get; set; } = new();

    /// <summary>
    /// Reference to game catalog for definition lookups.
    /// </summary>
    public IGameCatalog Catalog { get; }

    public SpawnWorkflowContext(IReadOnlyList<SpawnRequest> requests, IGameCatalog catalog)
    {
        Requests = requests;
        Catalog = catalog;
    }
}
