namespace TurnForge.Engine.Tests.Services.Queries;

using NUnit.Framework;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Tests.Helpers;
using TurnForge.Engine.ValueObjects;

/// <summary>
/// Unit tests for GameStateQueryService covering all query methods.
/// </summary>
[TestFixture]
public class GameStateQueryServiceTests
{
    [Test]
    public void GetAgent_ExistingAgent_ReturnsAgent()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var agentId, name: "TestSurvivor", category: "Player")
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var agent = query.GetAgent(agentId);
        
        // Assert
        Assert.That(agent, Is.Not.Null);
        Assert.That(agent!.Name, Is.EqualTo("TestSurvivor"));
        Assert.That(agent.Category, Is.EqualTo("Player"));
    }
    
    [Test]
    public void GetAgent_NonExistingAgent_ReturnsNull()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var agent = query.GetAgent("non-existing-id");
        
        // Assert
        Assert.That(agent, Is.Null);
    }
    
    [Test]
    public void GetProp_ExistingProp_ReturnsProp()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithProp("crate", out var propId, name: "WoodenCrate")
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var prop = query.GetProp(propId);
        
        // Assert
        Assert.That(prop, Is.Not.Null);
        Assert.That(prop!.Name, Is.EqualTo("WoodenCrate"));
    }
    
    [Test]
    public void GetProp_NonExistingProp_ReturnsNull()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var prop = query.GetProp("non-existing-id");
        
        // Assert
        Assert.That(prop, Is.Null);
    }
    
    [Test]
    public void GetAgentsAt_MultipleAgentsAtPosition_ReturnsAll()
    {
        // Arrange
        var sharedPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor1", out var _, position: sharedPosition)
            .WithAgent("survivor2", out var _, position: sharedPosition)
            .WithAgent("zombie", out var _, position: sharedPosition)
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var agents = query.GetAgentsAt(sharedPosition);
        
        // Assert
        Assert.That(agents, Has.Count.EqualTo(3));
    }
    
    [Test]
    public void GetAgentsAt_NoAgentsAtPosition_ReturnsEmpty()
    {
        // Arrange
        var position1 = Position.FromTile(new TileId(Guid.NewGuid()));
        var position2 = Position.FromTile(new TileId(Guid.NewGuid()));
        
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var _, position: position1)
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var agents = query.GetAgentsAt(position2); // Different position
        
        // Assert
        Assert.That(agents, Is.Empty);
    }
    
    [Test]
    public void GetAgentsAt_EmptyPosition_ReturnsEmpty()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var _)
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var agents = query.GetAgentsAt(Position.Empty);
        
        // Assert
        Assert.That(agents, Is.Empty);
    }
    
    [Test]
    public void GetAgentsByCategory_SurvivorCategory_ReturnsOnlySurvivors()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor1", out var _, category: "Survivor")
            .WithAgent("survivor2", out var _, category: "Survivor")
            .WithAgent("zombie", out var _, category: "Zombie")
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var survivors = query.GetAgentsByCategory("Survivor");
        
        // Assert
        Assert.That(survivors, Has.Count.EqualTo(2));
        Assert.That(survivors.All(a => a.Category == "Survivor"), Is.True);
    }
    
    [Test]
    public void GetAgentsByCategory_CaseInsensitive_Works()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var _, category: "Survivor")
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var survivors = query.GetAgentsByCategory("survivor"); // lowercase
        
        // Assert
        Assert.That(survivors, Has.Count.EqualTo(1));
    }
    
    [Test]
    public void GetAgentsByCategory_NonExistingCategory_ReturnsEmpty()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var _, category: "Survivor")
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var enemies = query.GetAgentsByCategory("Enemy");
        
        // Assert
        Assert.That(enemies, Is.Empty);
    }
    
    [Test]
    public void GetAgentsByCategory_NullOrEmpty_ReturnsEmpty()
    {
        // Arrange
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithAgent("survivor", out var _, category: "Survivor")
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var emptyResult = query.GetAgentsByCategory("");
        var nullResult = query.GetAgentsByCategory(null!);
        
        // Assert
        Assert.That(emptyResult, Is.Empty);
        Assert.That(nullResult, Is.Empty);
    }
    
    [Test]
    public void GetPropsAt_MultiplePropsAtPosition_ReturnsAll()
    {
        // Arrange
        var sharedPosition = Position.FromTile(new TileId(Guid.NewGuid()));
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithProp("crate1", out var _, position: sharedPosition)
            .WithProp("crate2", out var _, position: sharedPosition)
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var props = query.GetPropsAt(sharedPosition);
        
        // Assert
        Assert.That(props, Has.Count.EqualTo(2));
    }
    
    [Test]
    public void GetPropsAt_NoPropsAtPosition_ReturnsEmpty()
    {
        // Arrange
        var position1 = Position.FromTile(new TileId(Guid.NewGuid()));
        var position2 = Position.FromTile(new TileId(Guid.NewGuid()));
        
        var (state, _) = new TestGameBuilder()
            .WithBoard()
            .WithProp("crate", out var _, position: position1)
            .Build();
        
        var query = new GameStateQueryService(state);
        
        // Act
        var props = query.GetPropsAt(position2); // Different position
        
        // Assert
        Assert.That(props, Is.Empty);
    }
}
