using TurnForge.Engine.APIs;
using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Metrics;
using TurnForge.Engine.Core.Registries;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Interfaces;
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

using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Appliers; // ADDED

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
    private IActionRegistry? _workflowRegistry;
    
    // Definition-based setup
    private IBoardDefinition? _boardDefinition;
    private List<SpawnEntityOperation>? _initialEntities;
    private TurnOrderState? _turnOrder;
    private IEnumerable<BaseGameEntityDefinition>? _definitions;
    
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

    public GameEngineFactory WithActionRegistry(IActionRegistry registry)
    {
        _workflowRegistry = registry;
        return this;
    }

    [Obsolete("Use WithDefinitions to register descriptors and a GameStart workflow to initialize the board.")]
    public GameEngineFactory WithBoardDefinition(IBoardDefinition validBoardDefinition)
    {
        _boardDefinition = validBoardDefinition;
        return this;
    }

    [Obsolete("Use a GameStart workflow to spawn initial entities.")]
    public GameEngineFactory WithInitialEntities(List<SpawnEntityOperation> entities)
    {
        _initialEntities = entities;
        return this;
    }

    [Obsolete("Use a GameStart workflow to set turn order.")]
    public GameEngineFactory WithTurnOrder(TurnOrderState turnOrder)
    {
        _turnOrder = turnOrder;
        return this;
    }

    public GameEngineFactory WithDefinitions(IEnumerable<BaseGameEntityDefinition> definitions)
    {
        _definitions = definitions;
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
        
        // Register definitions if provided
        if (_definitions != null)
        {
            foreach (var def in _definitions)
            {
                gameCatalog.RegisterDefinition(def);
            }
        }
        
        services.RegisterSingleton<IGameCatalog>(gameCatalog);

// ... (This replace is tricky with existing content, let's use multi_replace or just target the bad block and then add import at top separately)
// Actually, let's just fix the bad block first to remove the misplaced using, then add using at top.

        services.RegisterSingleton<IGameEntityFactory>(
            new GenericEntityFactory(
                services.Resolve<IGameCatalog>(),
                services.Resolve<TraitInitializationService>()
            ));
        
        // Register EntityApplier
        services.RegisterSingleton<IEntityApplier>(new EntityApplier(
            services.Resolve<IGameCatalog>(),
            services.Resolve<TraitInitializationService>(),
            services.Resolve<IGameEntityFactory>()
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
        var workflowOrchestrator = new ActionOrchestrator();
        var workflowRegistry = _workflowRegistry ?? new ActionRegistry();
        
        workflowRegistry.Register(Core.Actions.CoreActions.StartGame, () => 
            Commands.StartGame.Action.StartGameAction.Create(
                services.Resolve<IBoardFactory>(), 
                services.Resolve<IEntityApplier>()
            ));
            
        services.RegisterSingleton<IActionRegistry>(workflowRegistry);

        // Build runtime
        var runtime = new GameEngineRuntime(
            commandBus, 
            repository, 
            orchestrator, 
            workflowOrchestrator,
            workflowRegistry,
            logger,
            boardFactory);
        
        // AUTO-INITIALIZATION FROM DEFINITIONS
        // If board definition is provided, create the initial state automatically.
        if (_boardDefinition != null)
        {
            var board = boardFactory.CreateGameBoard(_boardDefinition);
            var initialState = new Entities.GameState(
                System.Collections.Immutable.ImmutableDictionary<EntityId, GameEntity>.Empty,
                System.Collections.Immutable.ImmutableDictionary<PlayerId, Definitions.Actors.Player>.Empty,
                null,
                board,
                null,
                _turnOrder
            );
            
            // Apply initial entities if any
            if (_initialEntities != null && _initialEntities.Any())
            {
                var overlay = new GameStateOverlay(initialState);
                foreach (var op in _initialEntities)
                {
                    overlay.Record(op);
                }
                initialState = overlay.Commit();
            }
            
            repository.SaveGameState(initialState);
            orchestrator.SetState(initialState);
        }

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