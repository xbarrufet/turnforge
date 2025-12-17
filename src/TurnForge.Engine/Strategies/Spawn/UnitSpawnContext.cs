using TurnForge.Engine.Commands.GameStart.Definitions;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Strategies.Spawn;

public sealed record UnitSpawnContext(
    IReadOnlyList<UnitDescriptor> PlayerUnits,
    IReadOnlyGameState GameState);
