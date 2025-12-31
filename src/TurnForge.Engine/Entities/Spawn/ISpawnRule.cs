using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;

namespace TurnForge.Engine.Entities.Spawn;

/// <summary>
/// Defines a spawn rule that determines WHEN and WHAT entities spawn.
/// Game developers implement this interface for each spawn trigger type.
/// </summary>
/// <example>
/// public class EndOfRoundZombieSpawn : ISpawnRule
/// {
///     public bool ShouldTrigger(GameStateView state) 
///         => state.CurrentPhase == Phase.EndRound;
///         
///     public IEnumerable&lt;SpawnInstruction&gt; GetInstructions(GameStateView state)
///     {
///         foreach (var spawnPoint in GetActiveSpawnPoints(state))
///             yield return new SpawnInstruction(zombieDefinition, spawnPoint.Position);
///     }
/// }
/// </example>
public interface ISpawnRule
{
    /// <summary>
    /// Unique identifier for this spawn rule.
    /// </summary>
    string RuleId { get; }
    
    /// <summary>
    /// Determines if this rule should execute based on current game state.
    /// </summary>
    bool ShouldTrigger(GameStateView state);
    
    /// <summary>
    /// Generates spawn instructions when triggered.
    /// Each instruction specifies what to spawn and where.
    /// </summary>
    IEnumerable<SpawnInstruction> GetInstructions(GameStateView state);
}
