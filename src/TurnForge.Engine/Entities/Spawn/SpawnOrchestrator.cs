using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Spawn;

/// <summary>
/// Engine component that evaluates spawn rules and creates entities.
/// Called at appropriate game phases (end of round, on trigger, etc.)
/// </summary>
public sealed class SpawnOrchestrator
{
    private readonly IEntityApplier _applier;
    private readonly List<ISpawnRule> _rules = new();
    
    public SpawnOrchestrator(IEntityApplier applier)
    {
        _applier = applier;
    }
    
    /// <summary>
    /// Register a spawn rule to be evaluated.
    /// </summary>
    public void RegisterRule(ISpawnRule rule)
    {
        _rules.Add(rule);
    }
    
    /// <summary>
    /// Register multiple spawn rules.
    /// </summary>
    public void RegisterRules(IEnumerable<ISpawnRule> rules)
    {
        _rules.AddRange(rules);
    }
    
    /// <summary>
    /// Evaluate all registered rules and generate spawn operations.
    /// Returns operations to be recorded in the GameStateOverlay.
    /// </summary>
    public IEnumerable<SpawnEntityOperation> EvaluateRules(GameStateView stateView)
    {
        foreach (var rule in _rules)
        {
            if (!rule.ShouldTrigger(stateView))
                continue;
                
            foreach (var instruction in rule.GetInstructions(stateView))
            {
                for (int i = 0; i < instruction.Count; i++)
                {
                    var operation = _applier.Apply(instruction.Definition, instruction.Position);
                    yield return operation;
                }
            }
        }
    }
    
    /// <summary>
    /// Evaluate rules and directly record to overlay.
    /// </summary>
    public void ExecuteSpawns(GameStateView stateView, GameStateOverlay overlay)
    {
        foreach (var operation in EvaluateRules(stateView))
        {
            overlay.Record(operation);
        }
    }
}
