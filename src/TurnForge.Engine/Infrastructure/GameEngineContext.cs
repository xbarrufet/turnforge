using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure;

public readonly struct GameEngineContext(
    IGameRepository gameRepository,
    IPropSpawnStrategy propSpawnStrategy,
    IUnitSpawnStrategy unitSpawnStrategy)
{
    public IGameRepository GameRepository { get; init; } = gameRepository;
    
    public IPropSpawnStrategy PropSpawnStrategy { get; init; } = propSpawnStrategy;
    public IUnitSpawnStrategy UnitSpawnStrategy { get; init; } = unitSpawnStrategy;
}
