namespace TurnForge.Engine.Traits.Interfaces;

/// <summary>
/// Legacy alias for IDataTrait.
/// Maintained for backwards compatibility.
/// New code should use IDataTrait directly.
/// </summary>
[Obsolete("Use IDataTrait instead")]
public interface IBaseTrait : IDataTrait
{
}
