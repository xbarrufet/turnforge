using NUnit.Framework;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Items;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;
using System.Collections.Immutable;

namespace TurnForge.Engine.Tests.Entities.Overlay;

[TestFixture]
public class GameStateOverlayTests
{
    /// <summary>
    /// a) Verify via GameStateView position is the NEW one while GameState still has OLD position
    /// </summary>
    [Test]
    public void Overlay_MoveTo_ViewReturnsNewPosition_BaseStateRetainsOld()
    {
        // Arrange - Create entity with initial position
        var entityId = EntityId.New();
        var initialPosition = new TestPosition(0, 0);
        var newPosition = new TestPosition(5, 5);
        
        // Create game state with board and entity
        var entity = new Item(entityId, "test-item", "Test Item", "items");
        var gameState = CreateGameStateWithEntityAtPosition(entity, initialPosition);
        
        // Create overlay and record move operation
        var overlay = new GameStateOverlay();
        overlay.Record(new MoveOperation(entityId, newPosition));
        
        // Create view (with overlay applied)
        var view = new GameStateView(gameState, overlay);
        
        // Act & Assert
        // a) View should return NEW position
        var viewPosition = view.GetPosition(entityId);
        Assert.That(viewPosition, Is.EqualTo(newPosition), "View should return the overlay (new) position");
        
        // Base state should still have OLD position
        var basePosition = gameState.Board?.SpatialIndex.GetEntityPosition(entityId);
        Assert.That(basePosition, Is.EqualTo(initialPosition), "Base state should retain old position");
    }
    
    /// <summary>
    /// b) Verify commit properly persists the new position
    /// </summary>
    [Test]
    public void Overlay_Commit_NewPositionIsPersisted()
    {
        // Arrange
        var entityId = EntityId.New();
        var initialPosition = new TestPosition(0, 0);
        var newPosition = new TestPosition(5, 5);
        
        var entity = new Item(entityId, "test-item", "Test Item", "items");
        var gameState = CreateGameStateWithEntityAtPosition(entity, initialPosition);
        
        var overlay = new GameStateOverlay();
        overlay.Record(new MoveOperation(entityId, newPosition));
        
        // Act - Commit the overlay
        var committedState = overlay.Commit(gameState);
        
        // Assert - Committed state should have new position
        var committedPosition = committedState.Board?.SpatialIndex.GetEntityPosition(entityId);
        Assert.That(committedPosition, Is.EqualTo(newPosition), "Committed state should have new position");
        
        // Original state should be unchanged (immutability)
        var originalPosition = gameState.Board?.SpatialIndex.GetEntityPosition(entityId);
        Assert.That(originalPosition, Is.EqualTo(initialPosition), "Original state should remain unchanged");
    }
    
    /// <summary>
    /// Additional: Verify multiple moves keep only the latest in view
    /// </summary>
    [Test]
    public void Overlay_MultipleMoves_ViewReturnsLatestPosition()
    {
        // Arrange
        var entityId = EntityId.New();
        var pos0 = new TestPosition(0, 0);
        var pos1 = new TestPosition(1, 1);
        var pos2 = new TestPosition(2, 2);
        var posFinal = new TestPosition(10, 10);
        
        var entity = new Item(entityId, "test-item", "Test Item", "items");
        var gameState = CreateGameStateWithEntityAtPosition(entity, pos0);
        
        var overlay = new GameStateOverlay();
        overlay.Record(new MoveOperation(entityId, pos1));
        overlay.Record(new MoveOperation(entityId, pos2));
        overlay.Record(new MoveOperation(entityId, posFinal));
        
        var view = new GameStateView(gameState, overlay);
        
        // Act
        var viewPosition = view.GetPosition(entityId);
        
        // Assert - Should return the LATEST position
        Assert.That(viewPosition, Is.EqualTo(posFinal), "View should return the latest move position");
    }
    
    // ===============================
    // Test Helpers
    // ===============================
    
    private GameState CreateGameStateWithEntityAtPosition(GameEntity entity, IBoardPosition position)
    {
        // Create a minimal board with spatial index
        var spatialIndex = new SpatialIndex();
        spatialIndex.Register(entity.Id, position);
        
        var board = new GameBoard(
            id: EntityId.New(),
            kind: BoardKind.Discrete,
            topology: new TestTopology(),
            spatialIndex: spatialIndex
        );
        
        var entities = new Dictionary<EntityId, GameEntity> { { entity.Id, entity } }.ToImmutableDictionary();
        
        return new GameState(entities, ImmutableDictionary<PlayerId, Player>.Empty, null, board);
    }
    
    /// <summary>
    /// Simple test position implementation
    /// </summary>
    private record TestPosition(int X, int Y) : IBoardPosition
    {
        public BoardPositionKind Kind => BoardPositionKind.Tile;
    }
    
    /// <summary>
    /// Minimal topology implementation for tests
    /// </summary>
    private class TestTopology : IBoardTopology
    {
        public IBoardTopology Clone() => new TestTopology();
        public bool IsValidPosition(IBoardPosition position) => true;
        public bool CanTraverse(IBoardPosition from, IBoardPosition to) => true;
        public int Distance(IBoardPosition from, IBoardPosition to) => 1;
    }
}
