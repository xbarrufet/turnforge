using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;

namespace TurnForge.Engine.Entities.Spawn;

/// <summary>
/// Instruction to spawn one or more entities at a specific position.
/// Created by ISpawnRule and consumed by SpawnOrchestrator.
/// </summary>
public readonly record struct SpawnInstruction
{
    /// <summary>
    /// The definition template for the entity to spawn.
    /// </summary>
    public required BaseGameEntityDefinition Definition { get; init; }
    
    /// <summary>
    /// Where to place the spawned entity.
    /// </summary>
    public required IBoardPosition Position { get; init; }
    
    /// <summary>
    /// Number of entities to spawn (default: 1).
    /// </summary>
    public int Count { get; init; } = 1;
    
    public SpawnInstruction() { }
    
    public SpawnInstruction(BaseGameEntityDefinition definition, IBoardPosition position, int count = 1)
    {
        Definition = definition;
        Position = position;
        Count = count;
    }
}
