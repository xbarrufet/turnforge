namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Stores a list of special rules or capabilities as strings.
/// Useful for rules that don't have a specific logic implementation yet.
/// </summary>
public class SpecialRulesTrait : BaseDataTrait
{
    public List<string> Rules { get; } = new();

    public SpecialRulesTrait(params string[] rules)
    {
        Rules.AddRange(rules);
    }
}
