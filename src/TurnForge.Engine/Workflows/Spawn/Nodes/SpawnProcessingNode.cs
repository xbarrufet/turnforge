using TurnForge.Engine.Core.Factories;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Workflows.Spawn.Nodes;

/// <summary>
/// Processes spawn requests into descriptors.
/// Converts SpawnRequest → AgentDescriptor using catalog definitions.
/// </summary>
public class SpawnProcessingNode : INode
{
    public NodeId Id { get; } = new("Spawn.Processing");
    public INode? NextNode { get; set; }

    public ValidationResult Validate(WorkflowContext context)
    {
        if (context is not SpawnWorkflowContext spawnContext)
        {
            return ValidationResult.CancelResult;
        }

        var descriptors = new List<AgentDescriptor>();

        foreach (var request in spawnContext.Requests)
        {
            var definition = spawnContext.Catalog.GetDefinition<BaseGameEntityDefinition>(request.DefinitionId);
            if (definition == null)
            {
                continue;
            }

            for (int i = 0; i < request.Count; i++)
            {
                var descriptor = DescriptorBuilder.Build<AgentDescriptor>(request, definition);
                descriptors.Add(descriptor);
            }
        }

        spawnContext.Descriptors = descriptors;
        return ValidationResult.OkResult;
    }
}
