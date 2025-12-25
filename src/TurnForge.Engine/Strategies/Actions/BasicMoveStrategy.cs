using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Components;
using TurnForge.Engine.Decisions.Actions;
using TurnForge.Engine.Services.Queries;

namespace TurnForge.Engine.Strategies.Actions;

/// <summary>
/// Basic movement strategy with fixed 1 AP cost.
/// </summary>
/// <remarks>
/// Validation rules:
/// - Agent must exist
/// - Target position must be valid
/// - Agent must have sufficient AP (if HasCost=true)
/// - Target position must be different from current
/// 
/// This is a simple strategy. For game-specific rules (e.g., Zombicide),
/// extend or replace with custom strategy.
/// </remarks>
public sealed class BasicMoveStrategy : IActionStrategy<MoveCommand>
{
    private readonly IGameStateQuery _query;
    
    public BasicMoveStrategy(IGameStateQuery query)
    {
        _query = query ?? throw new ArgumentNullException(nameof(query));
    }
    
    public ActionStrategyResult Execute(MoveCommand command, IActionContext context)
    {
        // 1. Get agent
        var agent = _query.GetAgent(command.AgentId);
        if (agent == null)
            return ActionStrategyResult.Failed($"Agent '{command.AgentId}' not found");
        
        // 2. Validate target position
        if (!context.Board.IsValid(command.TargetPosition))
            return ActionStrategyResult.Failed("Invalid target position");
        
        // 3. Check if already at target (no-op)
        if (agent.PositionComponent.CurrentPosition == command.TargetPosition)
            return ActionStrategyResult.Failed("Already at target position");
        
        // 4. Get ActionPoints component (may be null if agent doesn't have AP system)
        var apComponent = agent.GetComponent<BaseActionPointsComponent>();
        if (apComponent == null)
        {
            // Agent doesn't have AP - treat as having infinite AP
            apComponent = new BaseActionPointsComponent(0) 
            { 
                CurrentActionPoints = int.MaxValue 
            };
        }
        
        // 5. Calculate cost (fixed 1 AP for basic strategy)
        const int movementCost = 1;
        var finalCost = command.HasCost ? movementCost : 0;
        
        // 6. Check if can afford
        if (command.HasCost && !apComponent.CanAfford(finalCost))
        {
            return ActionStrategyResult.Failed(
                $"Insufficient Action Points (need {finalCost}, have {apComponent.CurrentActionPoints})"
            );
        }
        
        // 7. Build decision with component updates
        var decision = new ActionDecisionBuilder()
            .ForEntity(command.AgentId)
            .UpdateComponent(new BasePositionComponent(command.TargetPosition))
            .UpdateComponent(new BaseActionPointsComponent(apComponent.MaxActionPoints)
            {
                CurrentActionPoints = apComponent.CurrentActionPoints - finalCost
            })
            .Build();
        
        // 8. Return success with metadata
        return ActionStrategyResult.Success(decision)
            .WithMetadata(new ActionMetadata { ActionPointsCost = finalCost });
    }
}
