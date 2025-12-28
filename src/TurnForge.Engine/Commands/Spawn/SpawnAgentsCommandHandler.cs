using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Workflows.Spawn;

namespace TurnForge.Engine.Commands.Spawn;

/// <summary>
/// Handler for spawning agents using the SpawnWorkflow.
/// Pipeline: SpawnRequest → SpawnWorkflow → Decisions
/// </summary>
public sealed class SpawnAgentsCommandHandler : ICommandHandler<SpawnAgentsCommand>
{
    private readonly IGameCatalog _catalog;
    private readonly IWorkflowOrchestrator _orchestrator;

    public SpawnAgentsCommandHandler(
        IGameCatalog catalog,
        IWorkflowOrchestrator orchestrator)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _orchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
    }

    public CommandResult Handle(SpawnAgentsCommand command)
    {
        // Create workflow and context
        var workflow = new SpawnWorkflow();
        var context = new SpawnWorkflowContext(command.Requests, _catalog);

        // Execute workflow
        var result = _orchestrator.Execute(workflow, context);

        // Handle workflow result
        if (result.Status == WorkflowStatus.Cancelled)
        {
            return CommandResult.Fail("Spawn workflow cancelled");
        }

        if (result.Status == WorkflowStatus.Suspended)
        {
            // Spawn should not require user input
            return CommandResult.Fail("Spawn workflow suspended unexpectedly");
        }

        // Extract decisions from context
        var decisions = context.Decisions.ToArray();

        return CommandResult.Ok(
            decisions: decisions,
            tags: "AgentsSpawned"
        );
    }
}
