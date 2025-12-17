using TurnForge.Engine.Commands.GameStart.Definitions;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Strategies.Spawn;

public sealed class UnitSpawnContext(
    IReadOnlyList<UnitDescriptor> playerUnits,
    IReadOnlyGameState gameState,
    ISpawnApplier spawner)
{
    public IReadOnlyList<UnitDescriptor> PlayerUnits { get; } = playerUnits;
    public IReadOnlyGameState GameState { get; } = gameState;
    public ISpawnApplier Spawner { get; } = spawner;
}