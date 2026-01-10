using ParchisLudo.Rules.Board;
using ParchisLudo.Rules.Fsm;
using ParchisLudo.Rules.Fsm.Nodes;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Rules.Factory;

/// <summary>
/// Wrapper class that holds the Engine facade and a reference to the FSM Graph
/// (useful for testing/simulation hacks until game logic is fully event-driven).
/// </summary>
public class ParchisGame
{
    public TurnForge.Engine.Core.TurnForge EngineWrapper { get; }
    public IGameEngine Engine => EngineWrapper.Runtime;

    // Exposed for simulation hacks (AP consumption)
    private readonly FsmGraph _graph; // kept private if possible, but simulation needs Node access
    public ParchisTurnNode? TurnNode => _graph.GetNode(new NodeId("Turn")) as ParchisTurnNode;

    private ParchisGame(TurnForge.Engine.Core.TurnForge engineWrapper, FsmGraph graph)
    {
        EngineWrapper = engineWrapper;
        _graph = graph;
    }

    /// <summary>
    /// Creates a fully configured Parchís game using only high-level TurnForge definitions.
    /// Does not instantiate internal engine components manually.
    /// </summary>
    [Obsolete]
    public static ParchisGame Create(params PlayerId[] players)
    {
        // 1. Prepare Definitions
        var boardDef = ParchisBoardFactory.CreateDescriptor();
        var turnOrder = ParchisFsmFactory.CreateTurnOrder(players);
       // var initialEntities = CreateInitialEntities(players);


        // 3. Prepare FSM (External to Engine Factory currently, passed as RootNode)
        // Note: GameEngineFactory creates its own FSM Graph wrapping the nodes.
        // We keep the graph structure generator here.
        var fsmGraph = ParchisFsmFactory.CreateFsmGraph(players);

        // 4. Configure Engine using High-Level API
        var engineWrapper = GameEngineFactory.Create(fsmGraph.RootNode!)
            .WithBoardDefinition(boardDef)
            .WithTurnOrder(turnOrder)
        //    .WithInitialEntities(initialEntities)
        //    .WithActionFactory(new ParchisActionFactory()) // TODO: Fix - ParchisActionFactory is commented out
            .Build();

        return new ParchisGame(engineWrapper, fsmGraph);
    }

  
}
