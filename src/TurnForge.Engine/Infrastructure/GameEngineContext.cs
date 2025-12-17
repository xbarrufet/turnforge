using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure;

public readonly struct GameEngineContext(IActorFactory actorFactory, 
    IGameRepository gameRepository,
    IPropSpawnStrategy propSpawnStrategy
    )
{
    public IActorFactory ActorFactory { get; init;  } = actorFactory;
    public IGameRepository GameRepository { get; init;  } = gameRepository;
    public IPropSpawnStrategy PropSpawnStrategy { get; init;  } = propSpawnStrategy;
}