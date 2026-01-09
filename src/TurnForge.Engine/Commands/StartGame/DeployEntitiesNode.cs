using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.StartGame.Action;

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
    
    public ActionStepResult Execute(ActionContext context)
    {
        // 1. Deploy Props
        if (context.TryGet<List<PropDeployment>>("PendingPropDeployments", out var props))
        {
            foreach (var prop in props)
            {
                var op = _applier.Apply(prop.Definition, prop.Position);
                context.Overlay.Record(op);
            }
        }
        
        // 2. Deploy Agents
        if (context.TryGet<List<AgentDeployment>>("PendingAgentDeployments", out var agents))
        {
            foreach (var agent in agents)
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
        }
        
        return ActionStepResult.Success();
    }
}
