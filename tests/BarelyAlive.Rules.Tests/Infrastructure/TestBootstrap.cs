using BarelyAlive.Rules.Apis;
using BarelyAlive.Rules.Apis.Interfaces;
using BarelyAlive.Rules.Game;
using TurnForge.Engine.APIs.Interfaces;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Repositories.Interfaces;

namespace BarelyAlive.Rules.Tests.Infrastructure;

public class TestBootstrap
{
    private readonly TurnForge.Engine.Core.TurnForge _turnForge;
    
    public IGameCatalogApi GameCatalog => _turnForge.GameCatalog;
    public IBarelyAliveApis BarelyAliveApis { get; }
    
    // Internal generic accessor for specific assertions if needed
    public TurnForge.Engine.Core.TurnForge Engine => _turnForge;

    public IGameRepository GameRepository { get; }

    private TestBootstrap(
        TurnForge.Engine.Core.Interfaces.IGameLogger? logger,
        TurnForge.Engine.Strategies.Spawn.Interfaces.ISpawnStrategy<TurnForge.Engine.Entities.Actors.Descriptors.PropDescriptor>? propStrategy,
        TurnForge.Engine.Strategies.Spawn.Interfaces.ISpawnStrategy<TurnForge.Engine.Entities.Actors.Descriptors.AgentDescriptor>? agentStrategy)
    {
        var safeLogger = logger ?? new TurnForge.Engine.Infrastructure.ConsoleLogger();
        GameRepository = new BarelyAlive.Rules.Adapter.Repositories.InMemoryGameRepository();
        
        var context = new GameEngineContext(
            GameRepository,
            propStrategy, 
            agentStrategy,
            safeLogger
        );

        _turnForge = GameEngineFactory.Build(context);
        
        // Initialize FSM using the ACTUAL Game Flow
        var fsmController = BarelyAliveGameFlow.CreateController();
        _turnForge.Runtime.SetFsmController(fsmController);

        BarelyAliveApis = new BarelyAliveApis(_turnForge.Runtime, _turnForge.GameCatalog);
    }

    public static TestBootstrap CreateNewGame(
        TurnForge.Engine.Core.Interfaces.IGameLogger? logger = null,
        TurnForge.Engine.Strategies.Spawn.Interfaces.ISpawnStrategy<TurnForge.Engine.Entities.Actors.Descriptors.PropDescriptor>? propStrategy = null,
        TurnForge.Engine.Strategies.Spawn.Interfaces.ISpawnStrategy<TurnForge.Engine.Entities.Actors.Descriptors.AgentDescriptor>? agentStrategy = null)
    {
        var bootstrap = new TestBootstrap(logger, propStrategy, agentStrategy);
        bootstrap.RegisterGameDefinitions();
        return bootstrap;
    }

    private void RegisterGameDefinitions()
    {
        _turnForge.GameCatalog.RegisterTestEntities();
    }
}
