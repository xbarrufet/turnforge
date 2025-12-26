using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Strategies.Actions;
using TurnForge.Engine.Strategies.Interactions;
using TurnForge.Engine.Services.Dice.ValueObjects;

namespace TurnForge.Engine.Strategies.Pipelines;

/// <summary>
/// Base class for strategies that execute as multi-step pipelines.
/// Supports suspension for user input and node-based flow control.
/// </summary>
/// <typeparam name="TCommand">Type of command this pipeline handles</typeparam>
/// <remarks>
/// USAGE PATTERN:
/// 1. Override GetStartNode() to return the first node
/// 2. Override GetNode(id) to return nodes by ID
/// 3. Use ActionContext.Variables to pass data between nodes
/// 4. Nodes can SuspendFor() to request user input
/// 5. When resumed, execution continues from current node
/// 
/// Example:
/// <code>
/// public class CombatPipeline : PipelineStrategy&lt;AttackCommand&gt;
/// {
///     private readonly CheckRangeNode _checkRange;
///     private readonly RollToHitNode _rollHit;
///     private readonly ApplyDamageNode _applyDamage;
///     
///     protected override IInteractionNode GetStartNode() => _checkRange;
///     protected override IInteractionNode? GetNode(string id) => id switch
///     {
///         "CheckRange" => _checkRange,
///         "RollToHit" => _rollHit,
///         "ApplyDamage" => _applyDamage,
///         _ => null
///     };
/// }
/// </code>
/// </remarks>
public abstract class PipelineStrategy<TCommand> : IActionStrategy<TCommand>
    where TCommand : IActionCommand
{
    private readonly List<DiceRollResult> _rolls = new();
    
    /// <summary>
    /// Return the first node in the pipeline.
    /// </summary>
    protected abstract IInteractionNode GetStartNode();
    
    /// <summary>
    /// Return a node by its ID for resumption.
    /// </summary>
    protected abstract IInteractionNode? GetNode(string nodeId);
    
    /// <summary>
    /// Optional initialization before pipeline starts.
    /// Override to set up command-specific variables.
    /// </summary>
    protected virtual void Initialize(TCommand command, ActionContext context)
    {
        // Store command for nodes to access
        context.SetVariable("Command", command);
    }
    
    public StrategyResult Execute(TCommand command, ActionContext context)
    {
        // Initialize on first run
        if (context.CurrentNodeId == null)
        {
            Initialize(command, context);
        }
        
        // Get starting node (or resume from current)
        var node = context.CurrentNodeId != null
            ? GetNode(context.CurrentNodeId)
            : GetStartNode();
        
        while (node != null)
        {
            // Execute node
            var result = node.Execute(context);
            context.History.Add(node.NodeId);
            
            // Handle suspension
            if (result.Suspend && result.Request != null)
            {
                context.CurrentNodeId = node.NodeId;
                return StrategyResult.Suspended(result.Request);
            }
            
            // Handle commit (end of pipeline)
            if (result.Commit && result.Decisions != null)
            {
                context.CurrentNodeId = null;
                return _rolls.Count > 0
                    ? StrategyResult.Completed(result.Decisions).WithRolls(_rolls)
                    : StrategyResult.Completed(result.Decisions);
            }
            
            // Continue to next node
            if (result.NextNodeId != null)
            {
                node = GetNode(result.NextNodeId);
                if (node == null)
                {
                    return StrategyResult.Failed($"Node '{result.NextNodeId}' not found in pipeline");
                }
            }
            else
            {
                // End of pipeline without commit = cancelled/failed
                node = null;
            }
        }
        
        return StrategyResult.Failed("Pipeline ended without commit");
    }
    
    /// <summary>
    /// Add a roll result for UI animation.
    /// Called by nodes during execution.
    /// </summary>
    protected void AddRoll(DiceRollResult roll)
    {
        _rolls.Add(roll);
    }
}
