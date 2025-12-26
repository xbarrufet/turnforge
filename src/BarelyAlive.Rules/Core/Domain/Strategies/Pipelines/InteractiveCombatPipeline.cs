using TurnForge.Engine.Commands.Attack;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Decisions.Actions;
using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;
using TurnForge.Engine.Strategies.Interactions;
using TurnForge.Engine.Strategies.Pipelines;

namespace BarelyAlive.Rules.Core.Domain.Strategies.Pipelines;

/// <summary>
/// Interactive combat pipeline demonstrating the suspension system.
/// </summary>
/// <remarks>
/// Flow:
/// 1. ValidateRange → Check if target is adjacent
/// 2. RequestHitRoll → SUSPEND for user to roll dice
/// 3. EvaluateHit → Check if hit succeeded
/// 4. ApplyDamage → Commit damage decision
/// 
/// Usage for testing:
/// - Strategy returns Suspended with InteractionRequest
/// - UI displays dice roller
/// - User rolls, UI sends SubmitInteractionCommand
/// - Strategy resumes from EvaluateHit node
/// </remarks>
public class InteractiveCombatPipeline : PipelineStrategy<AttackCommand>
{
    private readonly IGameStateQuery _query;
    private readonly IDiceThrowService _dice;
    
    // Nodes
    private readonly ValidateRangeNode _validateRange;
    private readonly RequestHitRollNode _requestHitRoll;
    private readonly EvaluateHitNode _evaluateHit;
    private readonly ApplyDamageNode _applyDamage;
    
    public InteractiveCombatPipeline(IGameStateQuery query, IDiceThrowService dice)
    {
        _query = query;
        _dice = dice;
        
        _validateRange = new ValidateRangeNode(_query);
        _requestHitRoll = new RequestHitRollNode();
        _evaluateHit = new EvaluateHitNode(_dice);
        _applyDamage = new ApplyDamageNode(_query);
    }
    
    protected override IInteractionNode GetStartNode() => _validateRange;
    
    protected override IInteractionNode? GetNode(string nodeId) => nodeId switch
    {
        "ValidateRange" => _validateRange,
        "RequestHitRoll" => _requestHitRoll,
        "EvaluateHit" => _evaluateHit,
        "ApplyDamage" => _applyDamage,
        _ => null
    };
}

/// <summary>
/// Validates attacker and target are in melee range.
/// </summary>
public class ValidateRangeNode : IInteractionNode
{
    private readonly IGameStateQuery _query;
    
    public string NodeId => "ValidateRange";
    
    public ValidateRangeNode(IGameStateQuery query)
    {
        _query = query;
    }
    
    public NodeResult Execute(ActionContext context)
    {
        var command = context.GetVariable<AttackCommand>("Command")!;
        
        var attacker = _query.GetAgent(command.AgentId);
        var target = _query.GetAgent(command.TargetId);
        
        if (attacker == null || target == null)
        {
            context.SetVariable("Error", "Attacker or target not found");
            return NodeResult.End();
        }
        
        // Store for later nodes
        context.SetVariable("Attacker", attacker);
        context.SetVariable("Target", target);
        
        // For now, skip distance check (would need Board in context)
        return NodeResult.Continue("RequestHitRoll");
    }
}

/// <summary>
/// Suspends to request user to roll dice.
/// </summary>
public class RequestHitRollNode : IInteractionNode
{
    public string NodeId => "RequestHitRoll";
    
    public NodeResult Execute(ActionContext context)
    {
        var command = context.GetVariable<AttackCommand>("Command")!;
        
        var request = new InteractionRequest
        {
            SessionId = context.SessionId,
            Type = "DiceRoll",
            Prompt = "Roll to hit! (Need 4+ on D6)",
            AgentId = command.AgentId,
            Metadata = new Dictionary<string, object>
            {
                ["DiceType"] = "1D6",
                ["Threshold"] = 4
            }
        };
        
        return NodeResult.SuspendFor(request);
    }
}

/// <summary>
/// Evaluates the hit roll after user input.
/// </summary>
public class EvaluateHitNode : IInteractionNode
{
    private readonly IDiceThrowService _dice;
    
    public string NodeId => "EvaluateHit";
    
    public EvaluateHitNode(IDiceThrowService dice)
    {
        _dice = dice;
    }
    
    public NodeResult Execute(ActionContext context)
    {
        // Get user's roll from variables (set by resume handler)
        var userRoll = context.GetVariable<int>("UserRoll", 0);
        
        // If no user roll, use dice service
        if (userRoll == 0)
        {
            var roll = _dice.Roll("1D6", "4+");
            userRoll = roll.Total;
            context.SetVariable("HitRoll", roll);
        }
        
        if (userRoll >= 4)
        {
            context.SetVariable("Hit", true);
            return NodeResult.Continue("ApplyDamage");
        }
        else
        {
            context.SetVariable("Hit", false);
            context.SetVariable("Error", "Attack missed!");
            return NodeResult.End();
        }
    }
}

/// <summary>
/// Applies damage to target and commits decisions.
/// </summary>
public class ApplyDamageNode : IInteractionNode
{
    private readonly IGameStateQuery _query;
    
    public string NodeId => "ApplyDamage";
    
    public ApplyDamageNode(IGameStateQuery query)
    {
        _query = query;
    }
    
    public NodeResult Execute(ActionContext context)
    {
        var command = context.GetVariable<AttackCommand>("Command")!;
        var target = _query.GetAgent(command.TargetId)!;
        
        // Fixed damage for demo
        const int damage = 2;
        
        var newHealth = new BaseHealthComponent(target.HealthComponent.MaxHealth)
        {
            CurrentHealth = target.HealthComponent.CurrentHealth - damage
        };
        
        var decisions = new List<ActionDecision>
        {
            new ActionDecisionBuilder()
                .ForEntity(target.Id.ToString())
                .UpdateComponent(newHealth)
                .Build()
        };
        
        // Also deduct AP from attacker
        var attacker = _query.GetAgent(command.AgentId)!;
        var ap = attacker.GetComponent<BaseActionPointsComponent>();
        if (ap != null)
        {
            decisions.Add(new ActionDecisionBuilder()
                .ForEntity(attacker.Id.ToString())
                .UpdateComponent(new BaseActionPointsComponent(ap.MaxActionPoints)
                {
                    CurrentActionPoints = ap.CurrentActionPoints - 1
                })
                .Build());
        }
        
        return NodeResult.CommitWith(decisions);
    }
}
