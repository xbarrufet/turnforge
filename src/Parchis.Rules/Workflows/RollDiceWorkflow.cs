using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Builders;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Board;

namespace Parchis.Rules.Workflows;

// ============================================================================
// Typed Workflow Data
// ============================================================================

/// <summary>
/// Typed data for dice roll results.
/// </summary>
public record RollDiceData(
    int Die1,
    int Die2,
    int Total,
    bool IsDouble,
    bool ExtraRoll,
    bool PenaltyThreeSixes
) : IWorkflowData;

// ============================================================================
// Workflow (using builder)
// ============================================================================

/// <summary>
/// Factory to create the RollDice workflow.
/// </summary>
public static class RollDiceWorkflowFactory
{
    public static IWorkflow Create()
    {
        return WorkflowBuilder.Create("Parchis.RollDice")
            .AddNode(new RollDiceNode())
            .Build();
    }
}

// ============================================================================
// Node
// ============================================================================

/// <summary>
/// Node that performs the dice roll.
/// </summary>
public class RollDiceNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        // Roll dice
        var random = new Random();
        var die1 = random.Next(1, 7);
        var die2 = random.Next(1, 7);
        
        // Check three sixes rule
        var consecutiveSixes = context.State.Metadata.TryGetValue("ConsecutiveSixes", out var cs) ? (int)cs : 0;
        var isThreeSixes = die1 == 6 && die2 == 6 && consecutiveSixes >= 2;
        
        // Store typed data (instead of string keys!)
        context.SetTypedData(new RollDiceData(
            Die1: die1,
            Die2: die2,
            Total: die1 + die2,
            IsDouble: die1 == die2,
            ExtraRoll: die1 == die2 && !isThreeSixes,
            PenaltyThreeSixes: isThreeSixes
        ));
        
        // Mark dice as rolled in state
        var newState = context.State
            .WithMetadata("DiceRolled", true)
            .WithMetadata("LastRoll.Die1", die1)
            .WithMetadata("LastRoll.Die2", die2)
            .WithMetadata("LastRoll.Total", die1 + die2);
        
        // Update consecutive sixes
        if (die1 == 6 && die2 == 6)
        {
            newState = newState.WithMetadata("ConsecutiveSixes", consecutiveSixes + 1);
        }
        else
        {
            newState = newState.WithMetadata("ConsecutiveSixes", 0);
        }
        
        context.RecordDecision(new UpdateStateDecision(newState));
        
        return ValidationResult.OkResult;
    }
}

/// <summary>
/// Simple decision to update state metadata.
/// </summary>
public class UpdateStateDecision : IDecision
{
    private readonly GameState _newState;
    
    public UpdateStateDecision(GameState newState)
    {
        _newState = newState;
    }
    
    public DecisionTiming Timing => DecisionTiming.Immediate;
    public string OriginId => "Parchis.UpdateState";
    
    public GameState Apply(GameState state) => _newState;
}
