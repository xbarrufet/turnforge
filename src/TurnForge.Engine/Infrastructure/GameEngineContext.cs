using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;

namespace TurnForge.Engine.Infrastructure;

public readonly struct GameContext(IActorFactory actorFactory, IGameRepository gameRepository)
{
    public IActorFactory ActorFactory { get; init;  } = actorFactory;
    public IGameRepository GameRepository { get; init;  } = gameRepository;
}