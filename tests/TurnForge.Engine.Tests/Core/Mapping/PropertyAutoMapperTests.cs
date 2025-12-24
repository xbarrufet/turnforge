using NUnit.Framework;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Core.Mapping;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Core.Mapping;

[TestFixture]
public class PropertyAutoMapperTests
{
    // --- Test Components ---

    public class TestComponent : IGameEntityComponent
    {
        public int Health { get; set; }
        
        [DoNotMap]
        public int InternalState { get; set; }
        
        public string Name { get; set; } = "";
    }

    public class AnotherComponent : IGameEntityComponent
    {
        public int Mana { get; set; }
        public int RenamedProperty { get; set; } // Targeted via explicit attribute
    }

    // --- Test Descriptors ---

    public class TestDescriptor
    {
        public int Health { get; set; }
        public int InternalState { get; set; } // Should be ignored due to [DoNotMap] on component
        public string Name { get; set; } = "";
        
        public int IgnoredProperty { get; set; } // No matching component property
    }

    public class ExplicitDescriptor
    {
        [MapToComponent(typeof(AnotherComponent), "RenamedProperty")]
        public int SourceValue { get; set; }
    }

    // --- Mocks ---

    public class MockEntity : GameEntity
    {
        public MockEntity() : base(EntityId.New(), "Mock", "Test", "def_1") 
        {
        }
    }

    [Test]
    public void Map_ImplicitlyMapsMatchingProperties()
    {
        // Arrange
        var entity = new MockEntity();
        var component = new TestComponent();
        entity.AddComponent(component);

        var descriptor = new TestDescriptor 
        { 
            Health = 100, 
            Name = "MappedName" 
        };

        // Act
        PropertyAutoMapper.Map(descriptor, entity);

        // Assert
        Assert.That(component.Health, Is.EqualTo(100));
        Assert.That(component.Name, Is.EqualTo("MappedName"));
    }

    [Test]
    public void Map_RespectsDoNotMapAttribute()
    {
        // Arrange
        var entity = new MockEntity();
        var component = new TestComponent();
        component.InternalState = 5; // Initial value
        entity.AddComponent(component);

        var descriptor = new TestDescriptor 
        { 
            InternalState = 999 
        };

        // Act
        PropertyAutoMapper.Map(descriptor, entity);

        // Assert
        Assert.That(component.InternalState, Is.EqualTo(5), "Should not map properties marked with [DoNotMap]");
    }

    [Test]
    public void Map_ExplicitlyMapsWithAttribute()
    {
        // Arrange
        var entity = new MockEntity();
        var component = new AnotherComponent();
        entity.AddComponent(component);

        var descriptor = new ExplicitDescriptor 
        { 
            SourceValue = 42 
        };

        // Act
        PropertyAutoMapper.Map(descriptor, entity);

        // Assert
        Assert.That(component.RenamedProperty, Is.EqualTo(42));
    }

    [Test]
    public void Map_IgnoresPropertiesWithoutMatch()
    {
        // Arrange
        var entity = new MockEntity();
        var component = new TestComponent();
        entity.AddComponent(component);

        var descriptor = new TestDescriptor 
        { 
            IgnoredProperty = 123 
        };

        // Act
        PropertyAutoMapper.Map(descriptor, entity);

        // Assert
        // No exception should be thrown, and nothing effectively happens
        Assert.Pass();
    }
}
