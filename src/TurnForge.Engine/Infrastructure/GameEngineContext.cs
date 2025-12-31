using TurnForge.Engine.Core.Metrics;
using TurnForge.Engine.Infrastructure.Persistence;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Repositories.Interfaces;

namespace TurnForge.Engine.Infrastructure;

/// <summary>
/// Context for GameEngine initialization.
/// All dependencies have sensible defaults for quick prototyping.
/// </summary>
public readonly struct GameEngineContext
{
    public IGameRepository GameRepository { get; init; }
    public TurnForge.Engine.Core.Interfaces.IGameLogger Logger { get; init; }
    public IEngineMetrics Metrics { get; init; }
    
    public GameEngineContext(
        IGameRepository? gameRepository = null,
        TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null,
        IEngineMetrics? metrics = null)
    {
        GameRepository = gameRepository ?? new InMemoryGameRepository();
        Logger = logger ?? new ConsoleLogger();
        Metrics = metrics ?? NullMetrics.Instance;
    }
    
    /// <summary>
    /// Creates context with all defaults. Zero config needed.
    /// </summary>
    public static GameEngineContext Default() => new();
}

public record GameEngineContextBuilder(
    IGameRepository? gameRepository = null,
    TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null,
    IEngineMetrics? metrics = null
)
{
    public GameEngineContext Build() => new(gameRepository, logger, metrics);

    public GameEngineContextBuilder WithLogger(TurnForge.Engine.Core.Interfaces.IGameLogger logger)
        => new(gameRepository, logger, metrics);

    public GameEngineContextBuilder WithMetrics(IEngineMetrics metrics)
        => new(gameRepository, logger, metrics);

    public GameEngineContextBuilder WithGameRepository(IGameRepository gameRepository)
        => new(gameRepository, logger, metrics);
}
