using TurnForge.Engine.Commands.Attack;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Decisions.Actions;
using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.Dice.ValueObjects;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Actions;

/// <summary>
/// Basic melee attack strategy for BarelyAlive.
/// Demonstrates combat using DiceThrowService with roll history.
/// </summary>
/// <remarks>
/// Combat flow:
/// 1. Validate attacker and target (range check)
/// 2. Roll to hit (based on attacker stats)
/// 3. Roll damage (based on weapon or unarmed)
/// 4. Apply damage to target
/// 5. Return decisions + roll history for UI
/// 
/// This is a simple MVP implementation. Add armor, special 
/// abilities, etc. as needed.
/// </remarks>
public sealed class BasicMeleeAttackStrategy : IActionStrategy<AttackCommand>
{
    private readonly IGameStateQuery _queryService;
    private readonly IDiceThrowService _dice;
    
    public BasicMeleeAttackStrategy(
        IGameStateQuery queryService,
        IDiceThrowService dice)
    {
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        _dice = dice ?? throw new ArgumentNullException(nameof(dice));
    }
    
    public StrategyResult Execute(AttackCommand command, ActionContext context)
    {
        var rolls = new List<DiceRollResult>();
        
        // 1. Validate attacker exists
        var attacker = _queryService.GetAgent(command.AgentId);
        if (attacker == null)
            return StrategyResult.Failed("Attacker not found");
        
        // 2. Validate target exists
        var target = _queryService.GetAgent(command.TargetId);
        if (target == null)
            return StrategyResult.Failed("Target not found");
        
        // 3. Range check - melee requires adjacent
        var attackerPos = attacker.PositionComponent.CurrentPosition;
        var targetPos = target.PositionComponent.CurrentPosition;
        var distance = context.Board.Distance(attackerPos, targetPos);
        
        if (distance > 1)
            return StrategyResult.Failed("Target out of melee range");
        
        // 4. AP check
        var apComponent = attacker.GetComponent<IActionPointsComponent>();
        if (apComponent != null && apComponent.CurrentActionPoints < 1)
            return StrategyResult.Failed("Not enough AP to attack");
        
        // 5. Get weapon stats (if any)
        var weaponDamage = 1; // Default unarmed damage
        var weaponHitBonus = 0;
        
        if (!string.IsNullOrEmpty(command.WeaponId))
        {
            var weapon = _queryService.GetItem(command.WeaponId);
            if (weapon != null)
            {
                var dmg = weapon.GetAttribute<int>("Damage");
                var hit = weapon.GetAttribute<int>("HitBonus");
                weaponDamage = dmg != 0 ? dmg : 1;
                weaponHitBonus = hit;
            }
        }
        
        // 6. Roll to hit (D6, need 4+ to hit)
        var hitRoll = _dice.Roll($"1D6+{weaponHitBonus}", "4+");
        rolls.Add(hitRoll);
        
        if (hitRoll.Pass != true)
        {
            // Miss - return with roll history
            return StrategyResult.Failed("Attack missed!")
                .WithRolls(rolls);
        }
        
        // 7. Roll damage
        var damageRoll = _dice.Roll($"1D6+{weaponDamage - 1}");
        rolls.Add(damageRoll);
        
        // 8. Apply damage to target
        var targetHealth = target.HealthComponent;
        var newHealth = new BaseHealthComponent(targetHealth.MaxHealth)
        {
            CurrentHealth = targetHealth.CurrentHealth - damageRoll.Total
        };
        
        // 9. Build decisions
        var decisions = new List<ActionDecision>();
        
        // Target takes damage
        var damageDecision = new ActionDecisionBuilder()
            .ForEntity(target.Id.ToString())
            .UpdateComponent(newHealth)
            .Build();
        decisions.Add(damageDecision);
        
        // Attacker spends AP
        if (apComponent != null)
        {
            var baseAp = attacker.GetComponent<BaseActionPointsComponent>();
            if (baseAp != null)
            {
                var apDecision = new ActionDecisionBuilder()
                    .ForEntity(attacker.Id.ToString())
                    .UpdateComponent(new BaseActionPointsComponent(baseAp.MaxActionPoints)
                    {
                        CurrentActionPoints = baseAp.CurrentActionPoints - 1
                    })
                    .Build();
                decisions.Add(apDecision);
            }
        }
        
        // 10. Return success with rolls
        return StrategyResult.Completed(decisions)
            .WithRolls(rolls)
            .WithMetadata(new ActionMetadata { ActionPointsCost = 1 });
    }
}
