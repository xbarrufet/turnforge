using TurnForge.Engine.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Components;

[TestFixture]
public class ItemComponentTests
{
    [Test]
    public void Constructor_SetsCategory()
    {
        var component = new ItemComponent("Weapon");
        
        Assert.That(component.Category, Is.EqualTo("Weapon"));
        Assert.That(component.OwnerId, Is.Null);
    }
    
    [Test]
    public void Constructor_WithOwner_SetsOwnerAndCategory()
    {
        var ownerId = EntityId.New();
        var component = new ItemComponent("Weapon", ownerId);
        
        Assert.That(component.Category, Is.EqualTo("Weapon"));
        Assert.That(component.OwnerId, Is.EqualTo(ownerId));
    }
    
    [Test]
    public void WithOwner_ReturnsNewInstanceWithOwner()
    {
        var component = new ItemComponent("Weapon");
        var ownerId = EntityId.New();
        
        var newComponent = component.WithOwner(ownerId);
        
        Assert.That(newComponent.OwnerId, Is.EqualTo(ownerId));
        Assert.That(newComponent.Category, Is.EqualTo("Weapon"));
        // Original unchanged
        Assert.That(component.OwnerId, Is.Null);
    }
    
    [Test]
    public void WithoutOwner_ReturnsNewInstanceWithoutOwner()
    {
        var ownerId = EntityId.New();
        var component = new ItemComponent("Weapon", ownerId);
        
        var newComponent = component.WithoutOwner();
        
        Assert.That(newComponent.OwnerId, Is.Null);
        Assert.That(newComponent.Category, Is.EqualTo("Weapon"));
    }
}
