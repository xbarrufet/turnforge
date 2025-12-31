using TurnForge.Engine.APIs;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Metrics;
using TurnForge.Engine.Core.Registries;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Infrastructure.Factories;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Infrastructure.Persistence;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Services;
using TurnForge.Engine.Definitions.Factories.Interfaces;

namespace TurnForge.Engine.Infrastructure;

/// <summary>
/// Builder for creating the TurnForge engine.
/// FSM root node is required; everything else has sensible defaults.
/// </summary>
/// <example>
/// var engine = GameEngineFactory.Create(myRootNode)
///     .WithLogger(myLogger)
///     .WithRepository(myRepo)
///     .Build();
/// </example>
public class GameEngineFactory
{
    private readonly IFsmNode _rootNode;
    private IGameRepository? _repository;
    private IGameLogger? _logger;
    private IEngineMetrics? _metrics;
    private IServiceProvider? _services;
    
    private GameEngineFactory(IFsmNode rootNode)
    {
        _rootNode = rootNode;
    }
    
    /// <summary>
    /// Start building with the required FSM root node.
    /// </summary>
    public static GameEngineFactory Create(IFsmNode rootNode)
    {
        return new GameEngineFactory(rootNode);
    }
    
    public GameEngineFactory WithRepository(IGameRepository repository)
    {
        _repository = repository;
        return this;
    }
    
    public GameEngineFactory WithLogger(IGameLogger logger)
    {
        _logger = logger;
        return this;
    }
    
    public GameEngineFactory WithMetrics(IEngineMetrics metrics)
    {
        _metrics = metrics;
        return this;
    }
    
    public GameEngineFactory WithServices(IServiceProvider services)
    {
        _services = services;
        return this;
    }
    
    /// <summary>
    /// Build the TurnForge engine with configured options.
    /// </summary>
    public Core.TurnForge Build()
    {
        // Apply defaults
        var repository = _repository ?? new InMemoryGameRepository();
        var logger = _logger ?? new ConsoleLogger();
        var metrics = _metrics ?? NullMetrics.Instance;
        
        // Initialize registries
        EntityTypeRegistry.Initialize();

        // Internal services
        var services = new SimpleServiceProvider();
        services.RegisterSingleton<TraitInitializationService>(new TraitInitializationService());
        services.RegisterSingleton<IGameFactory>(new SimpleGameFactory());

        var gameCatalog = new InMemoryGameCatalog();
        services.RegisterSingleton<IGameCatalog>(gameCatalog);

        services.RegisterSingleton<IGameEntityFactory>(
            new GenericEntityFactory(
                services.Resolve<IGameCatalog>(),
                services.Resolve<TraitInitializationService>()
            ));
        
        // Board infrastructure
        var topologyFactory = new BoardTopologyFactory();
        var spatialIndexFactory = new SpatialIndexFactory();
        services.RegisterSingleton<IBoardFactory>(new BoardFactory(topologyFactory, spatialIndexFactory));

        services.RegisterSingleton(repository);
        EngineCommandRegistration.Register(services);

        var resolver = new ServiceProviderCommandHandlerResolver(services);
        var commandBus = new CommandBus(resolver);
        IOrchestrator orchestrator = new SimpleOrchestrator();
        var boardFactory = services.Resolve<IBoardFactory>();
        var workflowOrchestrator = new WorkflowOrchestrator();

        // Build runtime
        var runtime = new GameEngineRuntime(
            commandBus, 
            repository, 
            orchestrator, 
            workflowOrchestrator,
            logger,
            boardFactory);
        
        // Create and set FSM graph
        var fsmGraph = new FsmGraph(_rootNode, _services ?? services, logger);
        runtime.SetFsmGraph(fsmGraph);
        
        var catalogApi = new GameCatalogApi(gameCatalog);
        return new Core.TurnForge(runtime, catalogApi);
    }
}

/// <summary>
/// Placeholder orchestrator until decision system is reimplemented.
/// </summary>
internal class SimpleOrchestrator : IOrchestrator
{
    private Entities.GameState _state = Entities.GameState.Empty();

    public Entities.GameState CurrentState => _state;

    public void SetState(Entities.GameState state) => _state = state;

    public void Enqueue(IEnumerable<Entities.Decisions.IDecision> decisions)
    {
        // TODO: Implement decision queue
    }

    public IEnumerable<IGameEvent> Apply(Entities.Decisions.IDecision decision)
    {
        // TODO: Implement decision application
        return Array.Empty<IGameEvent>();
    }

    public IEnumerable<IGameEvent> ExecuteScheduled(object? context, string hook)
    {
        // TODO: Implement scheduled execution
        return Array.Empty<IGameEvent>();
    }
}