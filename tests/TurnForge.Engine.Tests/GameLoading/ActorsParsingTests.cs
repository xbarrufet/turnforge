using NUnit.Framework;
using TurnForge.Engine.Tests.GameLoading;

namespace TurnForge.Engine.Tests.GameLoading
{
    [TestFixture]
    public sealed class ActorsParsingTests
    {
        [Test]
        public void MissionLoader_Binds_Actors_With_Traits_And_Optional_Id()
        {
            // Arrange
            var dto = MissionLoader.LoadFromFile("Assets/mission01.json");

            // Act & Assert
            Assert.That(dto.Actors, Is.Not.Null, "Actors collection should be present");
            Assert.That(dto.Actors.Count, Is.GreaterThan(0), "Actors collection should have at least one actor");

            var first = dto.Actors[0];
            Assert.That(first.ActorKind, Is.Not.Null.And.Not.Empty, "ActorKind should be bound");
            Assert.That(first.Position, Is.Not.Null.And.Not.Empty, "Position (GUID string) should be bound");
            Assert.That(first.CustomType, Is.Null.Or.Not.Empty, "CustomType may be present; if present should be non-empty");
            Assert.That(first.Traits, Is.Not.Null, "Traits collection should be present (can be empty)");

            // If there is a second actor, assert it may have actorId and traits with attributes
            if (dto.Actors.Count > 1)
            {
                var second = dto.Actors[1];
                Assert.That(second.ActorId, Is.Not.Null.And.Not.Empty, "Second actor expected to have actorId present");
                Assert.That(second.CustomType, Is.Not.Null.And.Not.Empty, "Second actor customType should be present");
                Assert.That(second.Traits, Is.Not.Null, "Traits should be present");
                Assert.That(second.Traits.Count, Is.GreaterThan(0), "Second actor should have at least one trait");

                var trait = second.Traits[0];
                Assert.That(trait.Type, Is.Not.Null.And.Not.Empty, "Trait type should be bound");
                Assert.That(trait.Attributes, Is.Not.Null, "Trait attributes should be present");
                Assert.That(trait.Attributes.Count, Is.GreaterThan(0), "Trait should have at least one attribute");

                var attr = trait.Attributes[0];
                Assert.That(attr.Name, Is.Not.Null.And.Not.Empty, "Attribute name should be bound");
                Assert.That(attr.Value, Is.Not.Null.And.Not.Empty, "Attribute value should be bound");
            }
        }
    }
}
