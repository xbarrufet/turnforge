using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers;

/// <summary>
/// Modifies existing entities by creating component operations.
/// </summary>
/// <typeparam name="TInput">Type of modification data</typeparam>
public interface IComponentApplier<in TInput>
{
    /// <summary>
    /// Apply modification to existing entity.
    /// </summary>
    /// <param name="target">Entity to modify</param>
    /// <param name="data">Modification data</param>
    /// <returns>Operation to record in overlay</returns>
    IGameStateOperation Apply(EntityId target, TInput data);
}
