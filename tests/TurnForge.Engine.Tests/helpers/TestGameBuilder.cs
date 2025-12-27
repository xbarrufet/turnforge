using Moq;
using TurnForge.Engine.Components;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Helpers;

/// <summary>
/// Fluent builder for creating test GameState instances.
/// Simplifies test setup with readable, reusable API.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// var (state, agentId) = new TestGameBuilder()
///     .WithBoard()
///     .WithAgent("survivor", out var id, ap: 3)
///     .Build();
/// </code>
/// </remarks>
public class TestGameBuilder
{
    private GameState _state = GameState.Empty();
    private GameBoard? _board;
    private readonly List<Zone> _zones = new();
    
    /// <summary>
    /// Add a simple mocked board (all positions valid).
    /// </summary>
    public TestGameBuilder WithBoard()
    {
        var spatialMock = new Mock<ISpatialModel>();
        spatialMock.Setup(m => m.IsValidPosition(It.IsAny<Position>())).Returns(true);
        
        _board = new GameBoard(spatialMock.Object);
        return this;
    }
    
    /// <summary>
    /// Add a board with custom spatial model.
    /// </summary>
    public TestGameBuilder WithBoard(ISpatialModel spatialModel)
    {
        _board = new GameBoard(spatialModel);
        return this;
    }
    
    /// <summary>
    /// Add an agent to the game state.
    /// </summary>
    /// <param name="definitionId">Definition ID (e.g., "survivor_1")</param>
    /// <param name="agentId">Output parameter with the generated EntityId string</param>
    /// <param name="name">Agent name (optional, defaults to definitionId)</param>
    /// <param name="category">Agent category (default: "Player")</param>
    /// <param name="position">Position (optional, generates random if null)</param>
    /// <param name="ap">Action Points (optional, no AP component if null)</param>
    /// <param name="maxAp">Max Action Points (defaults to ap value)</param>
    public TestGameBuilder WithAgent(
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
    
    /// <summary>
    /// Add a prop to the game state.
    /// </summary>
    public TestGameBuilder WithProp(
        string definitionId,
        out string propId,
        string? name = null,
        string category = "Prop",
        Position? position = null)
   {
        var id = EntityId.New();
        propId = id.ToString();
        
        var prop = new Prop(id, definitionId, name ?? definitionId, category);
        
        // Set position on existing PositionComponent (Prop constructor creates empty one)
        if (position.HasValue)
        {
            prop.PositionComponent.CurrentPosition = position.Value;
        }
        
        _state = _state.WithProp(prop);
        return this;
    }
    
    /// <summary>
    /// Add a zone to the board.
    /// </summary>
    public TestGameBuilder WithZone(Zone zone)
    {
        if (_board == null)
            throw new InvalidOperationException("Call WithBoard() before adding zones");
        
        _zones.Add(zone);
        _board.AddZone(zone);
        return this;
    }
    
    /// <summary>
    /// Build and return the GameState and Board.
    /// </summary>
    public (GameState State, GameBoard Board) Build()
    {
        if (_board == null)
            WithBoard(); // Create default board if not specified
        
        _state = _state.WithBoard(_board!);
        return (_state, _board!);
    }
    
    /// <summary>
    /// Build and return only the GameState.
    /// </summary>
    public GameState BuildState()
    {
        return Build().State;
    }
}
