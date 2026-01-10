using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Spawn;

/// <summary>
/// Service responsible for spawning entities from definitions or descriptors.
/// Used by actions and workflows for runtime spawns and deployment.
/// </summary>
public interface ISpawnService
{
    /// <summary>
    /// Spawn entity from Definition + Position.
    /// Used for runtime spawns (e.g., Zombicide end-of-round).
    /// </summary>
    /// <param name="definition">Entity template</param>
    /// <param name="position">Where to place the entity</param>
    /// <returns>Operation to record in overlay</returns>
    SpawnEntityOperation Spawn(BaseGameEntityDefinition definition, IBoardPositionId position);

    /// <summary>
    /// Spawn entity from Descriptor + Position.
    /// Used for player deployment with custom loadout.
    /// </summary>
    /// <param name="descriptor">Entity specification with overrides</param>
    /// <param name="position">Where to place the entity</param>
    /// <returns>Operation to record in overlay</returns>
    SpawnEntityOperation Spawn(IGameEntityBuildDescriptor descriptor, IBoardPositionId position);
    
    /// <summary>
    /// Move existing actor to new position.
    /// Used for starting position setups
    /// </summary>
    /// <param name="actor">Actor to be positioned</param>
    /// <param name="position">Where to place the entity</param>
    /// <returns>Actor with a new position</returns>
    Actor PositionActor(Actor actor, IBoardPositionId position);
}