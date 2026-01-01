using TurnForge.Engine.Core;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors; // ADDED
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components;
using TurnForge.Engine.Infrastructure;
using Parchis.Rules.Board;
using Parchis.Rules.Fsm;
using Parchis.Rules.Fsm.Nodes;
using System.Collections.Generic;
using System.Linq;

namespace Parchis.Rules.Factory;

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
    public static ParchisGame Create(params PlayerId[] players)
    {
        // 1. Prepare Definitions
        var boardDef = ParchisBoardFactory.CreateDescriptor();
        var turnOrder = ParchisFsmFactory.CreateTurnOrder(players);
        var initialEntities = CreateInitialEntities(players);
        
        // 2. Prepare Action Registry
        var registry = new ActionRegistry();
        ParchisActionRegistration.Register(registry);
        
        // 3. Prepare FSM (External to Engine Factory currently, passed as RootNode)
        // Note: GameEngineFactory creates its own FSM Graph wrapping the nodes.
        // We keep the graph structure generator here.
        var fsmGraph = ParchisFsmFactory.CreateFsmGraph(players);

        // 4. Configure Engine using High-Level API
        var engineWrapper = GameEngineFactory.Create(fsmGraph.RootNode!)
            .WithBoardDefinition(boardDef)
            .WithTurnOrder(turnOrder)
            .WithInitialEntities(initialEntities)
            .WithActionRegistry(registry)
            .Build();

        return new ParchisGame(engineWrapper, fsmGraph);
    }
    
    private static List<SpawnEntityOperation> CreateInitialEntities(PlayerId[] players)
    {
        var ops = new List<SpawnEntityOperation>();
        
        // Connections
        foreach (var desc in ParchisMissionFactory.CreateConnectionDescriptors())
        {
            var connId = desc.DefinitionId ?? $"conn_{desc.From.Value}_{desc.To.Value}";
            var ent = new ConnectionEntity(EntityId.New(), connId, connId, desc.Category);
            var pos = ConnectionPosition.Between(desc.From.Value, desc.To.Value);
            ent.AddComponent(new BasePositionComponent { CurrentPosition = pos });
            if (!string.IsNullOrEmpty(desc.RestrictedToTeam))
                ent.AddComponent(new TeamComponent(new TurnForge.Engine.Traits.Standard.TeamTrait(desc.RestrictedToTeam, "System")));
            ops.Add(new SpawnEntityOperation(ent.Id, ent, pos));
        }
        
        // Pawns
        var playerMap = players.ToDictionary(p => p.Value, p => p);
        var activeColors = players.Select(p => p.Value).ToHashSet();
        
        foreach (var color in activeColors)
        {
            for (int i = 0; i < 4; i++)
            {
                var pawn = new Agent(EntityId.New(), $"pawn_{color.ToLower()}_{i}", $"{color} Pawn {i}", "Pawn");
                var spawnPos = new TilePosition(new TileId($"spawn_{color.ToLower()}"));
                pawn.SetPositionComponent(new BasePositionComponent { CurrentPosition = spawnPos });
                pawn.ReplaceComponent(new TeamComponent(new TurnForge.Engine.Traits.Standard.TeamTrait(color, "Player", playerMap[color])));
                pawn.ControllerId = playerMap[color].Value;
                ops.Add(new SpawnEntityOperation(pawn.Id, pawn, spawnPos));
            }
        }
        
        return ops;
    }
}
