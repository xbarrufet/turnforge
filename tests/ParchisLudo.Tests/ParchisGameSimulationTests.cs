using NUnit.Framework;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Action;
using Parchis.Rules.Board;
using Parchis.Rules.Factory;
using Parchis.Rules.Actions;

namespace Parchis.Rules.Tests.Integration;

[TestFixture]
public class ParchisGameSimulationTests
{
    /// <summary>
    /// Concrete ActionContext for testing purposes.
    /// </summary>
    private class TestActionContext : ActionContext
    {
        public TestActionContext(GameState state)
        {
            InitializeState(state);
        }
        
        public override object? GetResult() => null;
    }
    
    /// <summary>
    /// Concrete GameEntity for testing purposes.
    /// </summary>
    private class TestGameEntity : GameEntity
    {
        public TestGameEntity(EntityId id, string definitionId, string name, string category) 
            : base(id, name, category, definitionId)
        {
        }
    }

    [Test]
    public void Simulation_StartGame_SpawnRule_And_ForwardMove()
    {
        // ---------------------------------------------------------
        // 1. SETUP BOARD & STATE (Manual)
        // ---------------------------------------------------------
        
        // Create Board Definition & Topology
        var boardDef = ParchisBoardFactory.CreateDescriptor();
        // Extract connections from boardDef for TileGraph constructor
        var connections = boardDef.Edges.Select(e => (e.positionFrom, e.positionTo));
        var topology = new TileGraph(connections);
        var spatial = new SpatialIndex();
        var board = new GameBoard(EntityId.New(), BoardKind.Discrete, topology, spatial);

        // Create Initial Empty State with Board
        var state = GameState.Empty();
        state = new GameState(state.Entities, state.Players, state.CurrentStateId, board, null, state.TurnOrder);
        
        // Create Overlay for Setup
        var setupOverlay = new GameStateOverlay(state);

        // ---------------------------------------------------------
        // 2. SPAWN CONNECTIONS & PAWNS (Manual)
        // ---------------------------------------------------------
        
        // A) Connections: We need them for movement logic to work (paths)
        var connectionDescriptors = ParchisMissionFactory.CreateConnectionDescriptors();
        foreach (var desc in connectionDescriptors)
        {
            var connId = desc.DefinitionId ?? $"conn_{desc.From.Value}_{desc.To.Value}";
            var ent = new TestGameEntity(EntityId.New(), connId, connId, desc.Category);
            
            // Position
            var pos = ConnectionPosition.Between(desc.From.Value, desc.To.Value);
            ent.AddComponent(new BasePositionComponent { CurrentPosition = pos });
            
            // Add TeamTrait if connection is team-restricted
            if (!string.IsNullOrEmpty(desc.RestrictedToTeam))
            {
                ent.AddComponent(new TeamComponent(
                    new TurnForge.Engine.Traits.Standard.TeamTrait(desc.RestrictedToTeam, "System")));
            }
            
            setupOverlay.Record(new SpawnEntityOperation(ent.Id, ent, pos));
        }

        // B) Pawns: 4 Red Pawns at Spawn (Using Agent as per Entity Hierarchy)
        var pawns = new List<TurnForge.Engine.Entities.Actors.Agent>();
        for (int i = 0; i < 4; i++)
        {
            var pawn = new TurnForge.Engine.Entities.Actors.Agent(
                EntityId.New(), $"pawn_red_{i}", $"Red Pawn {i}", "Agent");
            var spawnPos = new TilePosition(new TileId("spawn_red"));
            pawn.SetPositionComponent(new BasePositionComponent { CurrentPosition = spawnPos });
            pawn.ReplaceComponent(new TeamComponent(
                new TurnForge.Engine.Traits.Standard.TeamTrait("red", "Player", PlayerId.From("Red"))));
            pawn.ControllerId = "Red";
            
            setupOverlay.Record(new SpawnEntityOperation(pawn.Id, pawn, spawnPos));
            pawns.Add(pawn);
        }

        // Commit Setup
        state = setupOverlay.Commit();
        
        // ---------------------------------------------------------
        // 3. VERIFY INITIAL STATE
        // ---------------------------------------------------------
        var view = new GameStateView(state, new GameStateOverlay(state));
        
        // Verify connections exist
        var connsFromStart = view.GetConnectionsForTeam(new TileId("spawn_red"), "red").ToList();
        Assert.That(connsFromStart, Is.Not.Empty, "Must have connection from spawn_red");
        
        // Verify Pawns at spawn
        var pawnInSpawn = pawns[0];
        var initialPos = view.GetPosition(pawnInSpawn.Id) as TilePosition?;
        Assert.That(initialPos?.TileId.Value, Is.EqualTo("spawn_red"));

        // ---------------------------------------------------------
        // 4. EXECUTE MOVE WORKFLOW (Roll 5 -> Spawn Exit)
        // ---------------------------------------------------------
        
        // Prepare Context
        var context = new TestActionContext(state);
        context.Set("Roll", 5);
        context.Set("PlayerId", PlayerId.From("Red"));

        // Run RuleOfFiveNode
        var rule5Node = new RuleOfFiveNode();
        var res1 = rule5Node.Execute(context);
        Assert.That(res1.Status, Is.EqualTo(ActionStatus.Completed));

        // Verify MoveHandled
        Assert.That(context.Get<bool>("MoveHandled"), Is.True, "Rule of 5 should trigger");

        // Commit Move
        state = context.Overlay.Commit();
        view = new GameStateView(state, new GameStateOverlay(state));

        // Verify Position: One pawn should be at RedEntry
        var movedPawn = pawns.FirstOrDefault(p => 
        {
            var pos = view.GetPosition(p.Id) as TilePosition?;
            return pos?.TileId.Value == ParchisBoard.RedEntry;
        });
        
        Assert.That(movedPawn, Is.Not.Null, $"One pawn should be at {ParchisBoard.RedEntry}");
        
        // ---------------------------------------------------------
        // 5. EXECUTE MOVE WORKFLOW (Forward Move)
        // ---------------------------------------------------------
        
        // New Context with updated state
        context = new TestActionContext(state);
        context.Set("Roll", 3);
        context.Set("PlayerId", PlayerId.From("Red"));

        // Sequence: RuleOfFive (pass) -> SelectPawn -> ExecuteMove
        var node1 = new RuleOfFiveNode();
        node1.Execute(context);
        
        var node2 = new SelectPawnNode();
        var resSelect = node2.Execute(context);
        Assert.That(resSelect.Status, Is.EqualTo(ActionStatus.Completed));
        
        var node3 = new ExecuteMoveNode();
        var resExec = node3.Execute(context);
        Assert.That(resExec.Status, Is.EqualTo(ActionStatus.Completed));

        // Commit
        state = context.Overlay.Commit();
        view = new GameStateView(state, new GameStateOverlay(state));
        
        // Verify: Moved Forward from RedEntry (pawn is no longer at entry)
        var atEntry = view.GetPosition(movedPawn!.Id) as TilePosition?;
        Assert.That(atEntry?.TileId.Value, Is.Not.EqualTo(ParchisBoard.RedEntry), 
            "Pawn should have moved forward from entry");
    }
}
