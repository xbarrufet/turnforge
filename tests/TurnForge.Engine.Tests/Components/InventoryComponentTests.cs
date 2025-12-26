using TurnForge.Engine.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Components;

[TestFixture]
public class InventoryComponentTests
{
    [Test]
    public void Add_NewItem_AddsToItems()
    {
        var inventory = new InventoryComponent();
        var itemId = EntityId.New();
        
        inventory.Add(itemId);
        
        Assert.That(inventory.Items, Has.Count.EqualTo(1));
        Assert.That(inventory.Contains(itemId), Is.True);
    }
    
    [Test]
    public void Add_DuplicateItem_DoesNotAddAgain()
    {
        var inventory = new InventoryComponent();
        var itemId = EntityId.New();
        
        inventory.Add(itemId);
        inventory.Add(itemId);
        
        Assert.That(inventory.Items, Has.Count.EqualTo(1));
    }
    
    [Test]
    public void Remove_ExistingItem_ReturnsTrue()
    {
        var inventory = new InventoryComponent();
        var itemId = EntityId.New();
        inventory.Add(itemId);
        
        var result = inventory.Remove(itemId);
        
        Assert.That(result, Is.True);
        Assert.That(inventory.Contains(itemId), Is.False);
    }
    
    [Test]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        var inventory = new InventoryComponent();
        var itemId = EntityId.New();
        
        var result = inventory.Remove(itemId);
        
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void Count_ReturnsCorrectNumber()
    {
        var inventory = new InventoryComponent();
        inventory.Add(EntityId.New());
        inventory.Add(EntityId.New());
        inventory.Add(EntityId.New());
        
        Assert.That(inventory.Count, Is.EqualTo(3));
    }
    
    [Test]
    public void Empty_ReturnsEmptyInventory()
    {
        var inventory = InventoryComponent.Empty();
        
        Assert.That(inventory.Items, Is.Empty);
        Assert.That(inventory.Count, Is.EqualTo(0));
    }
}
