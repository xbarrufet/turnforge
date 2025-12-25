using Moq;
using TurnForge.Engine.Components;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Helpers;

/// <summary>
/// Fluent builder for creating test GameState instances.
/// Local copy for BarelyAlive tests.
/// </summary>
public class LocalTestGameBuilder
{
    private GameState _state = GameState.Empty();
    private GameBoard? _board;
    private readonly List<Zone> _zones = new();
    
    /// <summary>
    /// Add a simple mocked board (all positions valid).
    /// </summary>
    public LocalTestGameBuilder WithBoard()
    {
        var spatialMock = new Mock<ISpatialModel>();
        spatialMock.Setup(m => m.IsValidPosition(It.IsAny<Position>())).Returns(true);
        
        _board = new GameBoard(spatialMock.Object);
        return this;
    }
    
    /// <summary>
    /// Add a board with custom spatial model.
    /// </summary>
    public LocalTestGameBuilder WithBoard(ISpatialModel spatialModel)
    {
        _board = new GameBoard(spatialModel);
        return this;
    }
    
    public LocalTestGameBuilder WithAgent(
        string definitionId,
        out string agentId,
        string? name = null,
        string category = "Player",
        Position? position = null,
        int? ap = null,
        int? maxAp = null)
    {
        var id = EntityId.New();
        agentId = id.ToString();
        
        var agent = new Agent(id, definitionId, name ?? definitionId, category);
        
        // Set position on existing PositionComponent (Actor constructor creates empty one)
        var pos = position ?? Position.FromTile(new TileId(Guid.NewGuid()));
        agent.PositionComponent.CurrentPosition = pos;
        
        // Add AP component if specified
        if (ap.HasValue)
        {
            var maxApValue = maxAp ?? ap.Value;
            agent.AddComponent(new BaseActionPointsComponent(maxApValue) 
            { 
                CurrentActionPoints = ap.Value 
            });
        }
        
        _state = _state.WithAgent(agent);
        return this;
    }
    
    public LocalTestGameBuilder WithProp(
        string definitionId,
        out string propId,
        string? name = null,
        string category = "Prop",
        Position? position = null)
   {
        var id = EntityId.New();
        propId = id.ToString();
        
        var prop = new Prop(id, definitionId, name ?? definitionId, category);
        
        if (position.HasValue)
        {
            prop.PositionComponent.CurrentPosition = position.Value;
        }
        
        _state = _state.WithProp(prop);
        return this;
    }
    
    public LocalTestGameBuilder WithZone(Zone zone)
    {
        if (_board == null)
            throw new InvalidOperationException("Call WithBoard() before adding zones");
        
        _zones.Add(zone);
        _board.AddZone(zone);
        return this;
    }
    
    public (GameState State, GameBoard Board) Build()
    {
        if (_board == null)
            WithBoard(); // Create default board if not specified
        
        _state = _state.WithBoard(_board!);
        return (_state, _board!);
    }
    
    public GameState BuildState()
    {
        return Build().State;
    }
}
