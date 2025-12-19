using TurnForge.Engine.Descriptors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Strategies.Spawn;

public sealed class UnitSpawnContext(IReadOnlyList<UnitDescriptor> unitsToSpawn, GameState gameState)
    : ISpawnContext
{
    public GameState GameState { get; } = gameState;
    public IReadOnlyList<UnitDescriptor> UnitsToSpawn { get; } = unitsToSpawn;
}