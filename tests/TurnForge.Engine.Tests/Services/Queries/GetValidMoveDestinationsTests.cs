namespace TurnForge.Engine.Tests.Services.Queries;

using Moq;
using NUnit.Framework;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.Tests.Helpers;
using TurnForge.Engine.ValueObjects;

/// <summary>
/// Unit tests for GetValidMoveDestinations action query method.
/// </summary>
[TestFixture]
public class GetValidMoveDestinationsTests
{
    [Test]
    public void GetValidMoveDestinations_AgentWithSufficientAP_ReturnsNeighbors()
    {
        // Arrange
        var currentPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var neighbor1 = Position.FromTile(new TileId(Guid.NewGuid()));
        var neighbor2 = Position.FromTile(new TileId(Guid.NewGuid()));
        
        var spatialMock = new Mock<ISpatialModel>();
        spatialMock.Setup(m => m.GetNeighbors(currentPos))
            .Returns(new[] { neighbor1, neighbor2 });
        spatialMock.Setup(m => m.IsValidPosition(It.IsAny<Position>()))
            .Returns(true);
        
        var (state, board) = new TestGameBuilder()
            .WithBoard(spatialMock.Object)
            .WithAgent("survivor", out var agentId, position: currentPos, ap: 3)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        
        // Act
        var validMoves = query.GetValidMoveDestinations(agentId);
        
        // Assert
        Assert.That(validMoves, Has.Count.EqualTo(2));
        Assert.That(validMoves, Contains.Item(neighbor1));
        Assert.That(validMoves, Contains.Item(neighbor2));
    }
    
    [Test]
    public void GetValidMoveDestinations_AgentWith0AP_ReturnsEmpty()
    {
        // Arrange
        var currentPos = Position.FromTile(new TileId(Guid.NewGuid()));
        
        var (state, board) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var agentId, position: currentPos, ap: 0)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        
        // Act
        var validMoves = query.GetValidMoveDestinations(agentId);
        
        // Assert
        Assert.That(validMoves, Is.Empty);
    }
    
    [Test]
    public void GetValidMoveDestinations_NonExistentAgent_ReturnsEmpty()
    {
        // Arrange
        var (state, board) = new TestGameBuilder()
            .WithBoard()
            .Build();
        
        var query = new GameStateQueryService(state, board);
        
        // Act
        var validMoves = query.GetValidMoveDestinations("non-existent-id");
        
        // Assert
        Assert.That(validMoves, Is.Empty);
    }
    
    [Test]
    public void GetValidMoveDestinations_FiltersBoardBounds()
    {
        // Arrange
        var currentPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var validNeighbor = Position.FromTile(new TileId(Guid.NewGuid()));
        var invalidNeighbor = Position.FromTile(new TileId(Guid.NewGuid()));
        
        var spatialMock = new Mock<ISpatialModel>();
        spatialMock.Setup(m => m.GetNeighbors(currentPos))
            .Returns(new[] { validNeighbor, invalidNeighbor });
        spatialMock.Setup(m => m.IsValidPosition(validNeighbor)).Returns(true);
        spatialMock.Setup(m => m.IsValidPosition(invalidNeighbor)).Returns(false);
        
        var (state, board) = new TestGameBuilder()
            .WithBoard(spatialMock.Object)
            .WithAgent("survivor", out var agentId, position: currentPos, ap: 3)
            .Build();
        
        var query = new GameStateQueryService(state, board);
        
        // Act
        var validMoves = query.GetValidMoveDestinations(agentId);
        
        // Assert
        Assert.That(validMoves, Has.Count.EqualTo(1));
        Assert.That(validMoves, Contains.Item(validNeighbor));
        Assert.That(validMoves, Does.Not.Contain(invalidNeighbor));
    }
    
    [Test]
    public void GetValidMoveDestinations_AgentWithoutAPComponent_ReturnsEmpty()
    {
        // Arrange: Agent without AP component treats as 0 AP
        var currentPos = Position.FromTile(new TileId(Guid.NewGuid()));
        var neighbor = Position.FromTile(new TileId(Guid.NewGuid()));
        
        var spatialMock = new Mock<ISpatialModel>();
        spatialMock.Setup(m => m.GetNeighbors(currentPos))
            .Returns(new[] { neighbor });
        spatialMock.Setup(m => m.IsValidPosition(It.IsAny<Position>()))
            .Returns(true);
        
        var (state, board) = new TestGameBuilder()
            .WithBoard(spatialMock.Object)
            .WithAgent("survivor", out var agentId, position: currentPos, ap: null) // No AP component
            .Build();
        
        var query = new GameStateQueryService(state, board);
        
        // Act
        var validMoves = query.GetValidMoveDestinations(agentId);
        
        // Assert
        Assert.That(validMoves, Is.Empty, "Agent without AP component should be treated as having 0 AP");
    }
}
