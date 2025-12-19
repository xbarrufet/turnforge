using System.Text.Json;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Rules.BarelyAlive.Dto;
using TurnForge.Rules.BarelyAlive.Traits;
using NUnit.Framework;

namespace TurnForge.Rules.BarelyAlive.Test.Traits;

[TestFixture]
public class BarelyAliveTraitFactoryTests
{
    [Test]
    public void Create_SpawnOrder_ReturnsSpawnOrderTrait()
    {
        // Arrange
        var dto = new TraitDto
        {
            Type = "SpawnOrder",
            Attributes = new List<TraitAttributeDto> 
            { 
                new TraitAttributeDto { Name = "order", Value = "5" } 
            }
        };

        // Act
        var result = BarelyAliveTraitFactory.Create(dto);

        // Assert
        Assert.That(result, Is.InstanceOf<TraitTypes.SpawnOrderTrait>());
        Assert.That(((TraitTypes.SpawnOrderTrait)result).Order, Is.EqualTo(5));
    }
    
    [Test]
    public void Create_UnknownType_ThrowsNotSupportedException()
    {
        // Arrange
        var dto = new TraitDto
        {
            Type = "Unknown",
            Attributes = new List<TraitAttributeDto>()
        };

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => BarelyAliveTraitFactory.Create(dto));
    }
}
