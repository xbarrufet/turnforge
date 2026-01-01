using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Workflow;

/// <summary>
/// Final node in StartGame workflow that deploys all pending entities to the board.
/// Executes after board has been created.
/// </summary>
public class DeployEntitiesNode : INode
{
    private readonly IEntityApplier _applier;
    
    public NodeId Id { get; }
    public INode? NextNode { get; set; }
    
    public DeployEntitiesNode(NodeId id, IEntityApplier applier)
    {
        Id = id;
        _applier = applier;
    }
    
    public WorkflowStepResult Execute(WorkflowContext context)
    {
        var ctx = (StartGameWorkflowContext)context;
        
        // 1. Deploy Props (Definition + Fixed Position)
        foreach (var prop in ctx.PendingPropDeployments)
        {
            var op = _applier.Apply(prop.Definition, prop.Position);
            context.Overlay.Record(op);
        }
        
        // 2. Deploy Agents (Descriptor + Position)
        //    Position is either:
        //    - Explicit (Kill Team)
        //    - Resolved by mission rules (Zombicide)
        foreach (var agent in ctx.PendingAgentDeployments)
        {
            if (agent.Position == null)
            {
                throw new InvalidOperationException(
                    $"Deploy position not set for agent {agent.Descriptor.DefinitionId}. " +
                    "Position must be provided in input or resolved by mission rules.");
            }
            
            var op = _applier.Apply(agent.Descriptor, agent.Position);
            context.Overlay.Record(op);
        }
        
        return WorkflowStepResult.Success();
    }
}
