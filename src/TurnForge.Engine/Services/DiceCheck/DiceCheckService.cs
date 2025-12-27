using TurnForge.Engine.Services.Dice;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.Traits.Standard.Checkers;

namespace TurnForge.Engine.Services.DiceCheck;

/// <summary>
/// Default implementation of IDiceCheckService.
/// Resolves stat checks based on the check condition type.
/// </summary>
public class DiceCheckService : IDiceCheckService
{
    private readonly IDiceThrowService _diceService;
    
    public DiceCheckService(IDiceThrowService diceService)
    {
        _diceService = diceService;
    }
    
    /// <inheritdoc/>
    public CheckResult Check(CheckerStatTrait stat, CheckParams parameters)
    {
        // Roll the dice
        var rollResult = stat.DicePattern.IsRandom 
            ? stat.DicePattern.Resolve(_diceService)
            : stat.DicePattern.FixedValue;
        
        // Determine threshold based on condition type
        var threshold = ResolveThreshold(stat.Condition, parameters);
        
        // Check success
        var success = rollResult >= threshold;
        
        return new CheckResult(
            Success: success,
            RollValue: rollResult,
            RequiredThreshold: threshold,
            StatName: stat.StatName
        );
    }
    
    /// <summary>
    /// Resolves the required threshold based on condition type.
    /// </summary>
    private static int ResolveThreshold(ICheckCondition condition, CheckParams parameters)
    {
        return condition switch
        {
            FixedThreshold ft => ft.Value,
            
            OpposedCheck _ => parameters.OpposedValue 
                ?? throw new ArgumentException("OpposedCheck requires OpposedValue in parameters"),
            
            TableLookup tl => ResolveTableThreshold(tl, parameters),
            
            _ => throw new NotSupportedException($"Unknown check condition type: {condition.GetType().Name}")
        };
    }
    
    /// <summary>
    /// Resolves threshold via table lookup.
    /// </summary>
    private static int ResolveTableThreshold(TableLookup lookup, CheckParams parameters)
    {
        if (parameters.Rulebook == null)
            throw new ArgumentException("TableLookup requires Rulebook in parameters");
        
        // Two-value lookup (attacker vs defender)
        if (parameters.AttackerValue.HasValue && parameters.DefenderValue.HasValue)
        {
            return parameters.Rulebook.GetThreshold(
                lookup.TableName, 
                parameters.AttackerValue.Value, 
                parameters.DefenderValue.Value
            );
        }
        
        // Single-value lookup
        if (parameters.AttackerValue.HasValue)
        {
            return parameters.Rulebook.GetThreshold(
                lookup.TableName, 
                parameters.AttackerValue.Value
            );
        }
        
        throw new ArgumentException("TableLookup requires at least AttackerValue in parameters");
    }
}
