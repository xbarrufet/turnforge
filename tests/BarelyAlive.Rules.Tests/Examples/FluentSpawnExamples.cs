using NUnit.Framework;
using BarelyAlive.Rules.Tests.Infrastructure;
using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components.Interfaces;

namespace BarelyAlive.Rules.Tests.Examples;

/// <summary>
/// Demonstrates the improved DX using SpawnRequestBuilder.
/// Compares old vs new syntax side-by-side.
/// </summary>
[TestFixture]
public class FluentSpawnExamples
{
    private TestBootstrap _bootstrap = null!;
    private Position _playerSpawn;
    private Position _zombieSpawn;
    private Position _bossSpawn;

    [SetUp]
    public void Setup()
    {
        _bootstrap = TestBootstrap.CreateNewGame();
        
        // Setup mission board
        var (spatial, zones, props, agents) = BarelyAlive.Rules.Adapter.Loaders.MissionLoader
            .ParseMissionString(TestHelpers.Mission01Json);
        var boardDesc = new TurnForge.Engine.Definitions.Board.Descriptors.BoardDescriptor(spatial, zones);
        
        _bootstrap.Engine.Runtime.ExecuteCommand(new InitializeBoardCommand(boardDesc));
        _bootstrap.Engine.Runtime.ExecuteCommand(new TurnForge.Engine.Commands.ACK.ACKCommand());
        
        // Setup spawn positions
        _playerSpawn = new Position(new TileId(System.Guid.Parse("07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53"))); // (1,1)
        _zombieSpawn = new Position(new TileId(System.Guid.Parse("d7de841d-64a5-48b3-9662-0fe757a8950e"))); // (0,0)
        _bossSpawn = new Position(new TileId(System.Guid.Parse("8f3e5b2a-1c4d-4e9f-b7a3-6f2d8c9e1a0b"))); // Example
    }

    /// <summary>
    /// Example 1: Simple survivor spawn
    /// </summary>
    [Test]
    public void Example1_SimpleSpawn_BeforeAndAfter()
    {
        // ===== BEFORE (current syntax - works fine) =====
         var mikeRequestOld = SpawnRequestBuilder.For(TestHelpers.MikeId)
            .At(_playerSpawn)
            .Build();

        // ===== AFTER (fluent builder - more discoverable) =====
        var mikeRequestNew = SpawnRequestBuilder
            .For(TestHelpers.MikeId)
            .At(_playerSpawn)
            .Build();

        // Both produce identical SpawnRequest objects
        Assert.That(mikeRequestNew.DefinitionId, Is.EqualTo(mikeRequestOld.DefinitionId));
        // Verify Position Trait
        var traitOld = mikeRequestOld.TraitsToOverride!.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().First();
        var traitNew = mikeRequestNew.TraitsToOverride!.OfType<TurnForge.Engine.Traits.Standard.PositionTrait>().First();
        Assert.That(traitNew.InitialPosition, Is.EqualTo(traitOld.InitialPosition));
    }

    /// <summary>
    /// Example 2: Batch spawn (10 zombies)
    /// </summary>
    [Test]
    public void Example2_BatchSpawn_BeforeAndAfter()
    {
        // ===== AFTER =====
        var zombiesNew = SpawnRequestBuilder
            .For("Enemies.Zombie")
            .At(_zombieSpawn)
            .WithCount(10)
            .Build();

        // Verify
        Assert.That(zombiesNew.Count, Is.EqualTo(10));
        Assert.That(zombiesNew.DefinitionId, Is.EqualTo("Enemies.Zombie"));
    }

    /// <summary>
    /// Example 3: Implicit conversion for concise syntax
    /// </summary>
    [Test]
    public void Example3_ImplicitConversion_EvenMoreConcise()
    {
        // Can drop the .Build() call due to implicit conversion
        SpawnRequest survivor = SpawnRequestBuilder
            .For(TestHelpers.MikeId)
            .At(_playerSpawn);

        // Can use directly in command with explicit Build()
        var cmd = new SpawnAgentsCommand(new[]
        {
            SpawnRequestBuilder.For(TestHelpers.MikeId).At(_playerSpawn).Build(),
            SpawnRequestBuilder.For(TestHelpers.DougId).At(_playerSpawn).Build()
        });

        Assert.That(cmd.Requests.Count, Is.EqualTo(2));
    }
}
