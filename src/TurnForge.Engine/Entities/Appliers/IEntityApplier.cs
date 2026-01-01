using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Overlay;

namespace TurnForge.Engine.Entities.Appliers;

/// <summary>
/// Creates entities from definitions or descriptors.
/// Used by SpawnOrchestrator for runtime spawns and deployment.
/// </summary>
public interface IEntityApplier
{
    /// <summary>
    /// Create entity from Definition + Position.
    /// Used for runtime spawns (e.g., Zombicide end-of-round).
    /// </summary>
    /// <param name="definition">Entity template</param>
    /// <param name="position">Where to place the entity</param>
    /// <returns>Operation to record in overlay</returns>
    SpawnEntityOperation Apply(BaseGameEntityDefinition definition, IBoardPosition position);
    
    /// <summary>
    /// Create entity from Descriptor + Position.
    /// Used for player deployment with custom loadout.
    /// </summary>
    /// <param name="descriptor">Entity specification with overrides</param>
    /// <param name="position">Where to place the entity</param>
    /// <returns>Operation to record in overlay</returns>
    SpawnEntityOperation Apply(IGameEntityBuildDescriptor descriptor, IBoardPosition position);
}
