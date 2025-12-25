using TurnForge.Engine.Commands.Move;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Decisions.Actions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Actions;

/// <summary>
/// BarelyAlive (Zombicide) movement strategy.
/// Survivors pay extra AP for each zombie at their starting position.
/// </summary>
/// <remarks>
/// Movement cost calculation:
/// - Survivor: 1 AP + number of zombies at starting position
/// - Zombie: 1 AP (fixed)
/// 
/// Design: Extends engine's movement validation but customizes cost calculation.
/// Uses IGameStateQuery to count zombies at position.
/// </remarks>
public sealed class BarelyAliveMovementStrategy : IActionStrategy<MoveCommand>
{
    private readonly IGameStateQuery _queryService;
    
    public BarelyAliveMovementStrategy(IGameStateQuery queryService)
    {
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
    }
    
    public ActionStrategyResult Execute(MoveCommand command, IActionContext context)
    {
        // 1. Validate agent exists
        var agent = _queryService.GetAgent(command.AgentId);
        if (agent == null)
            return ActionStrategyResult.Failed("Agent not found");
        
        // 2. Validate target position
        if (!context.Board.IsValid(command.TargetPosition))
            return ActionStrategyResult.Failed("Invalid target position");
        
        // 3. Check if already at target position
        var currentPosition = agent.PositionComponent.CurrentPosition;
        if (currentPosition == command.TargetPosition)
            return ActionStrategyResult.Failed("Already at target position");
        
        // 4. Calculate BarelyAlive-specific cost
        var cost = CalculateMovementCost(agent, currentPosition);
        
        // 5. Validate AP if movement has cost
        if (command.HasCost)
        {
            var apComponent = agent.GetComponent<IActionPointsComponent>();
            if (apComponent != null && !apComponent.CanAfford(cost))
                return ActionStrategyResult.Failed("Insufficient Action Points");
        }
        
        // 6. Get AP component for update
        var currentApComponent = agent.GetComponent<BaseActionPointsComponent>();
        if (currentApComponent == null)
        {
            // Agent doesn't have AP - treat as having infinite AP
            currentApComponent = new BaseActionPointsComponent(0) 
            { 
                CurrentActionPoints = int.MaxValue  
            };
        }
        
        var finalCost = command.HasCost ? cost : 0;
        
        // 7. Build decision with component updates
        var decision = new ActionDecisionBuilder()
            .ForEntity(command.AgentId)
            .UpdateComponent(new BasePositionComponent(command.TargetPosition))
            .UpdateComponent(new BaseActionPointsComponent(currentApComponent.MaxActionPoints)
            {
                CurrentActionPoints = currentApComponent.CurrentActionPoints - finalCost
            })
            .Build();
        
        // 8. Return success with metadata
        return ActionStrategyResult.Success(decision)
            .WithMetadata(new ActionMetadata { ActionPointsCost = finalCost });
    }
    
    /// <summary>
    /// Calculate movement cost based on BarelyAlive rules.
    /// Survivors: 1 + zombies at starting position
    /// Zombies: 1 (fixed)
    /// </summary>
    private int CalculateMovementCost(GameEntity agent, Position startPosition)
    {
        // Base cost is always 1
        int baseCost = 1;
        
        // Check if agent is a Survivor (by category)
        if (agent.Category.Equals("Survivor", StringComparison.OrdinalIgnoreCase))
        {
            // Count OTHER zombies at starting position (exclude the moving agent)
            var agentsAtPosition = _queryService.GetAgentsAt(startPosition);
            var zombiesAtPosition = agentsAtPosition
                .Where(a => a.Id != agent.Id && a.Category.Equals("Zombie", StringComparison.OrdinalIgnoreCase))
                .Count();
            
            return baseCost + zombiesAtPosition;
        }
        
        // Zombies and other entities pay fixed cost
        return baseCost;
    }
}
