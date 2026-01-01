using NUnit.Framework;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Spawn;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Tests.Entities.Spawn;

[TestFixture]
public class ConnectionSpawnerTests
{
    [Test]
    public void CreatesConnectionEntity_WithCorrectProperties()
    {
        // Arrange
        var spawner = new ConnectionSpawner();
        var descriptor = new ConnectionDescriptor(
            From: new TileId("A"),
            To: new TileId("B"),
            Category: "forward"
        );
        
        // Act
        var result = spawner.CreateConnections(new[] { descriptor }).First();
        
        // Assert
        Assert.That(result, Is.InstanceOf<ConnectionEntity>());
        Assert.That(result.Category, Is.EqualTo("forward"));
        Assert.That(result.DefinitionId, Is.EqualTo("connection_forward_A_B"));
        
        var pos = result.GetComponent<IPositionComponent>()?.CurrentPosition as ConnectionPosition?;
        Assert.That(pos, Is.Not.Null);
        Assert.That(pos.Value.From.Value, Is.EqualTo("A"));
        Assert.That(pos.Value.To.Value, Is.EqualTo("B"));
    }
    
    [Test]
    public void CreatesRestrictedConnection_WithTeamComponent()
    {
        // Arrange
        var spawner = new ConnectionSpawner();
        var descriptor = new ConnectionDescriptor(
            From: new TileId("X"),
            To: new TileId("Y"),
            Category: "finish_entry",
            RestrictedToTeam: "red"
        );
        
        // Act
        var result = spawner.CreateConnections(new[] { descriptor }).First();
        
        // Assert
        var teamComponent = result.GetComponent<ITeamComponent>();
        Assert.That(teamComponent, Is.Not.Null);
        Assert.That(teamComponent.Team, Is.EqualTo("red"));
    }
}
