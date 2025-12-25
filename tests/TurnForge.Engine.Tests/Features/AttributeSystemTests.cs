using NUnit.Framework;
using Moq;
using TurnForge.Engine.Values;
using TurnForge.Engine.Components;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Features;

[TestFixture]
public class AttributeSystemTests
{
    [Test]
    public void DiceThrowType_Parse_ValidNotation_ReturnsObject()
    {
        var dice = DiceThrowType.Parse("2D6+3");
        
        Assert.That(dice.DiceCount, Is.EqualTo(2));
        Assert.That(dice.DiceSides, Is.EqualTo(6));
        Assert.That(dice.Modifier, Is.EqualTo(3));
        Assert.That(dice.ToString(), Is.EqualTo("2D6+3"));
    }

    [Test]
    public void DiceThrowType_Parse_SimpleNotation_ReturnsObject()
    {
        var dice = DiceThrowType.Parse("1D20");
        
        Assert.That(dice.DiceCount, Is.EqualTo(1));
        Assert.That(dice.DiceSides, Is.EqualTo(20));
        Assert.That(dice.Modifier, Is.EqualTo(0));
        Assert.That(dice.ToString(), Is.EqualTo("1D20"));
    }

    [Test]
    public void AttributeValue_IntConstructor_IsCorrect()
    {
        var val = new AttributeValue(5);
        
        Assert.That(val.BaseValue, Is.EqualTo(5));
        Assert.That(val.CurrentValue, Is.EqualTo(5));
        Assert.That(val.IsDiceAttribute, Is.False);
        Assert.That(val.ToString(), Is.EqualTo("5"));
    }

    [Test]
    public void AttributeValue_DiceConstructor_IsCorrect()
    {
        var dice = DiceThrowType.Parse("1D6");
        var val = new AttributeValue(dice);
        
        Assert.That(val.BaseValue, Is.EqualTo(0));
        Assert.That(val.IsDiceAttribute, Is.True);
        Assert.That(val.ToString(), Is.EqualTo("1D6"));
    }

    [Test]
    public void Factory_BuildsEntity_WithAttributes()
    {
        // GIVEN
        var catalogMock = new Mock<IGameCatalog>();
        var factory = new GenericActorFactory(catalogMock.Object);
        
        var definition = new BaseGameEntityDefinition
        {
            Attributes = new Dictionary<string, object>
            {
                { "Strength", 5 },
                { "Damage", "1D6+2" }
            }
        };

        catalogMock.Setup(c => c.GetDefinition<BaseGameEntityDefinition>("TestAgent"))
            .Returns(definition);

        var descriptor = new AgentDescriptor("TestAgent")
        { 
            Position = new Position(new TileId(Guid.NewGuid()))
        };

        // WHEN
        var agent = factory.BuildAgent(descriptor);

        // THEN
        var attrComponent = agent.GetComponent<AttributeComponent>();
        Assert.That(attrComponent, Is.Not.Null);
        
        var str = attrComponent.Get("Strength");
        Assert.That(str.HasValue, Is.True);
        Assert.That(str.Value.CurrentValue, Is.EqualTo(5));
        
        var dmg = attrComponent.Get("Damage");
        Assert.That(dmg.HasValue, Is.True);
        Assert.That(dmg.Value.IsDiceAttribute, Is.True);
        Assert.That(dmg.Value.Dice.ToString(), Is.EqualTo("1D6+2"));
    }
}
