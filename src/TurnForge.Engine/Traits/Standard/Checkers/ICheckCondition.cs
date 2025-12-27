namespace TurnForge.Engine.Traits.Standard.Checkers;

/// <summary>
/// Defines how a stat check should be validated.
/// </summary>
public interface ICheckCondition { }

/// <summary>
/// Fixed threshold: roll >= value (e.g., "4+", "5+").
/// </summary>
/// <param name="Value">Required minimum roll (e.g., 4 means "4+")</param>
public record FixedThreshold(int Value) : ICheckCondition;

/// <summary>
/// Opposed check: roll >= provided parameter value at check time.
/// </summary>
public record OpposedCheck() : ICheckCondition;

/// <summary>
/// Table lookup: use RulebookService to determine threshold.
/// </summary>
/// <param name="TableName">Name of the table to look up (e.g., "ToWound", "ArmorSave")</param>
public record TableLookup(string TableName) : ICheckCondition;
