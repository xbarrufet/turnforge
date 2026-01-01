using NUnit.Framework;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Logic;
using TurnForge.Engine.Entities.Overlay;

namespace Parchis.Rules.Tests.Logic;

[TestFixture]
public class ParchisMoveLogicTests
{
    private GameState _state;
    private GameStateView _view;
    
    [SetUp]
    public void Setup()
    {
        _state = GameState.Empty();
        _view = new GameStateView(_state, new GameStateOverlay(_state));
    }
    
    private void AddConnection(string from, string to, string category, string? team = null)
    {
        var entity = new ConnectionEntity(EntityId.New(), "conn", "conn", category);
        var pos = ConnectionPosition.Between(from, to);
        entity.AddComponent(new BasePositionComponent { CurrentPosition = pos });
        
        if (team != null)
        {
            var trait = new TurnForge.Engine.Traits.Standard.TeamTrait(team, "System");
            entity.AddComponent(new TeamComponent(trait));
        }
        
        // Correctly handle ImmutableDictionary
        var newEntities = _state.Entities.Add(entity.Id, entity);
        _state = new GameState(
            newEntities, 
            _state.Players, 
            _state.CurrentStateId, 
            _state.Board, 
            _state.Mission, 
            _state.TurnOrder);
            
        // Must recreate view because it points to old state instance
        _view = new GameStateView(_state, new GameStateOverlay(_state));
    }

    [Test]
    public void Move_StandardForward_ReturnsDestination()
    {
        // A -> B -> C -> D
        AddConnection("A", "B", "forward");
        AddConnection("B", "C", "forward");
        AddConnection("C", "D", "forward");
        
        // DEBUG: Deep inspect
        var conn = _state.Entities.Values.FirstOrDefault();
        if (conn != null)
        {
             var pos = conn.GetComponent<IPositionComponent>()?.CurrentPosition;
             Console.WriteLine($"Conn Pos: {pos}");
             if (pos is ConnectionPosition cp)
             {
                 Console.WriteLine($"From: '{cp.From.Value}', To: '{cp.To.Value}'");
                 Console.WriteLine($"Query 'A' == From? {new TileId("A") == cp.From}");
             }
        }
        
        // DEBUG: Check connections directly
        var connections = _view.GetConnectionsForTeam(new TileId("A"), "red").ToList();
        Console.WriteLine($"Found {connections.Count} connections from A");
        foreach(var c in connections) Console.WriteLine($" - {c.Id} ({c.Category})");
        
        var start = new TileId("A");
        var dest = ParchisMoveLogic.CalculateDestination(_view, start, 3, "red", out bool center, out bool bounce);
        
        Assert.That(dest?.Value, Is.EqualTo("D"));
        Assert.That(center, Is.False);
        Assert.That(bounce, Is.False);
    }
    
    [Test]
    public void Move_EntersFinishLane_IfColorMatches()
    {
        // track_68 -> red_finish_1 (finish_entry, red)
        AddConnection("track_68", "red_finish_1", "finish_entry", "red");
        
        var start = new TileId("track_68");
        var dest = ParchisMoveLogic.CalculateDestination(_view, start, 1, "red", out _, out _);
        
        Assert.That(dest?.Value, Is.EqualTo("red_finish_1"));
    }
    
    [Test]
    public void Move_DoesNotEnterFinishLane_IfColorMismatch()
    {
        // track_68 -> red_finish_1 (finish_entry, red)
        // track_68 -> track_1 (forward)
        AddConnection("track_68", "red_finish_1", "finish_entry", "red");
        AddConnection("track_68", "track_1", "forward"); // Alternative path
        
        var start = new TileId("track_68");
        // Blue player should take forward path, ignored finish_entry
        var dest = ParchisMoveLogic.CalculateDestination(_view, start, 1, "blue", out _, out _);
        
        Assert.That(dest?.Value, Is.EqualTo("track_1"));
    }
    
    [Test]
    public void Move_ReachesCenter_ExactSteps()
    {
        // red_finish_7 -> center (finish_complete)
        AddConnection("red_finish_7", "center", "finish_complete", "red");
        
        var start = new TileId("red_finish_7");
        var dest = ParchisMoveLogic.CalculateDestination(_view, start, 1, "red", out bool center, out _);
        
        Assert.That(dest?.Value, Is.EqualTo("center"));
        Assert.That(center, Is.True);
    }
    
    [Test]
    public void Move_BouncesBack_IfOvershoot()
    {
        // red_finish_7 -> center (finish_complete)
        AddConnection("red_finish_7", "center", "finish_complete", "red");
        
        // Start at 7. Roll 3.
        // Step 1: Center (remaining 2).
        // Bounce 1: red_finish_7. (remaining 1). (Back from center/last tile)
        // Bounce 2: red_finish_6. (remaining 0).
        
        // Logic inside BounceBack parses strings.
        
        var start = new TileId("red_finish_7");
        var dest = ParchisMoveLogic.CalculateDestination(_view, start, 3, "red", out bool center, out bool bounce);
        
        Assert.That(dest?.Value, Is.EqualTo("red_finish_6"));
        Assert.That(center, Is.False);
        Assert.That(bounce, Is.True);
    }
}
