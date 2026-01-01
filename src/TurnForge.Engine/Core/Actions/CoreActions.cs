using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Actions;

/// <summary>
/// ActionIds for engine-provided (Core) actions.
/// Core actions are automatically registered by the engine - games don't need to register them.
/// 
/// Usage:
///   engine.ExecuteAction(CoreActions.StartGame, parameters);
/// </summary>
public static class CoreActions
{
    /// <summary>
    /// Initializes the game: configures board, players, and spawns initial entities.
    /// This is typically the first action called after creating the engine.
    /// </summary>
    public static readonly ActionId StartGame = new("Core.StartGame");
    
    // Future core actions:
    // public static readonly ActionId Spawn = new("Core.Spawn");
}
