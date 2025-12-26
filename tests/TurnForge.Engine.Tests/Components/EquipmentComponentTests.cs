using TurnForge.Engine.Components;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Components;

[TestFixture]
public class EquipmentComponentTests
{
    [Test]
    public void Equip_NewSlot_SetsItem()
    {
        var equipment = new EquipmentComponent();
        var itemId = EntityId.New();
        
        equipment.Equip("Weapon", itemId);
        
        Assert.That(equipment.GetEquipped("Weapon"), Is.EqualTo(itemId));
    }
    
    [Test]
    public void Equip_ExistingSlot_ReplacesItem()
    {
        var equipment = new EquipmentComponent();
        var sword1 = EntityId.New();
        var sword2 = EntityId.New();
        
        equipment.Equip("Weapon", sword1);
        equipment.Equip("Weapon", sword2);
        
        Assert.That(equipment.GetEquipped("Weapon"), Is.EqualTo(sword2));
    }
    
    [Test]
    public void Unequip_ExistingSlot_ReturnsItemAndClearsSlot()
    {
        var equipment = new EquipmentComponent();
        var itemId = EntityId.New();
        equipment.Equip("Weapon", itemId);
        
        var result = equipment.Unequip("Weapon");
        
        Assert.That(result, Is.EqualTo(itemId));
        Assert.That(equipment.GetEquipped("Weapon"), Is.Null);
    }
    
    [Test]
    public void Unequip_EmptySlot_ReturnsNull()
    {
        var equipment = new EquipmentComponent();
        
        var result = equipment.Unequip("Weapon");
        
        Assert.That(result, Is.Null);
    }
    
    [Test]
    public void HasEquipped_WithItem_ReturnsTrue()
    {
        var equipment = new EquipmentComponent();
        equipment.Equip("Weapon", EntityId.New());
        
        Assert.That(equipment.HasEquipped("Weapon"), Is.True);
    }
    
    [Test]
    public void HasEquipped_WithoutItem_ReturnsFalse()
    {
        var equipment = new EquipmentComponent();
        
        Assert.That(equipment.HasEquipped("Weapon"), Is.False);
    }
    
    [Test]
    public void IsEquipped_ItemInSlot_ReturnsTrue()
    {
        var equipment = new EquipmentComponent();
        var itemId = EntityId.New();
        equipment.Equip("Weapon", itemId);
        
        Assert.That(equipment.IsEquipped(itemId), Is.True);
    }
    
    [Test]
    public void IsEquipped_ItemNotInAnySlot_ReturnsFalse()
    {
        var equipment = new EquipmentComponent();
        var itemId = EntityId.New();
        
        Assert.That(equipment.IsEquipped(itemId), Is.False);
    }
    
    [Test]
    public void GetSlotFor_EquippedItem_ReturnsSlotName()
    {
        var equipment = new EquipmentComponent();
        var itemId = EntityId.New();
        equipment.Equip("Armor", itemId);
        
        var slot = equipment.GetSlotFor(itemId);
        
        Assert.That(slot, Is.EqualTo("Armor"));
    }
    
    [Test]
    public void GetSlotFor_NotEquippedItem_ReturnsNull()
    {
        var equipment = new EquipmentComponent();
        var itemId = EntityId.New();
        
        var slot = equipment.GetSlotFor(itemId);
        
        Assert.That(slot, Is.Null);
    }
    
    [Test]
    public void Slots_ReturnsAllEquippedItems()
    {
        var equipment = new EquipmentComponent();
        equipment.Equip("Weapon", EntityId.New());
        equipment.Equip("Armor", EntityId.New());
        
        Assert.That(equipment.Slots, Has.Count.EqualTo(2));
    }
}
