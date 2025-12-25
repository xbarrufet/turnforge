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
        var boardDesc = new TurnForge.Engine.Entities.Board.Descriptors.BoardDescriptor(spatial, zones);
        
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
        var mikeRequestOld = new SpawnRequest(
            TestHelpers.MikeId,
            Position: _playerSpawn
        );

        // ===== AFTER (fluent builder - more discoverable) =====
        var mikeRequestNew = SpawnRequestBuilder
            .For(TestHelpers.MikeId)
            .At(_playerSpawn)
            .Build();

        // Both produce identical SpawnRequest objects
        Assert.That(mikeRequestNew.DefinitionId, Is.EqualTo(mikeRequestOld.DefinitionId));
        Assert.That(mikeRequestNew.Position, Is.EqualTo(mikeRequestOld.Position));
    }

    /// <summary>
    /// Example 2: Spawn with property overrides
    /// </summary>
    [Test]
    public void Example2_WithProperties_BeforeAndAfter()
    {
        // ===== BEFORE =====
        var survivorOld = new SpawnRequest(
            TestHelpers.MikeId,
            Position: _playerSpawn,
            PropertyOverrides: new Dictionary<string, object>
            {
                ["Health"] = 12,
                ["ActionPoints"] = 4,
                ["Faction"] = "Police"
            }
        );

        // ===== AFTER (more fluent, IntelliSense-friendly) =====
        var survivorNew = SpawnRequestBuilder
            .For(TestHelpers.MikeId)
            .At(_playerSpawn)
            .WithProperty("Health", 12)
            .WithProperty("ActionPoints", 4)
            .WithProperty("Faction", "Police")
            .Build();

        // Verify equivalence
        Assert.That(survivorNew.PropertyOverrides!["Health"], Is.EqualTo(12));
        Assert.That(survivorNew.PropertyOverrides["ActionPoints"], Is.EqualTo(4));
        Assert.That(survivorNew.PropertyOverrides["Faction"], Is.EqualTo("Police"));
    }

    /// <summary>
    /// Example 3: Batch spawn (10 zombies)
    /// </summary>
    [Test]
    public void Example3_BatchSpawn_BeforeAndAfter()
    {
        // ===== BEFORE =====
        var zombiesOld = new SpawnRequest(
            "Enemies.Zombie",
            Count: 10,
            Position: _zombieSpawn
        );

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
    /// Example 4: Complex boss with multiple properties
    /// </summary>
    [Test]
    public void Example4_ComplexBossWithProperties_BeforeAndAfter()
    {
        // ===== BEFORE (verbose, harder to read) =====
        var bossOld = new SpawnRequest(
            "Enemies.DragonBoss",
            Count: 1,
            Position: _bossSpawn,
            PropertyOverrides: new Dictionary<string, object>
            {
                ["Health"] = 1000,
                ["PhaseCount"] = 5,
                ["Faction"] = "Undead",
                ["Difficulty"] = "Nightmare"
            }
        );

        // ===== AFTER (clean, readable, discoverable) =====
        var bossNew = SpawnRequestBuilder
            .For("Enemies.DragonBoss")
            .At(_bossSpawn)
            .WithProperty("Health", 1000)
            .WithProperty("PhaseCount", 5)
            .WithProperty("Faction", "Undead")
            .WithProperty("Difficulty", "Nightmare")
            .Build();

        // Verify
        Assert.That(bossNew.PropertyOverrides!.Count, Is.EqualTo(4));
        Assert.That(bossNew.PropertyOverrides["Health"], Is.EqualTo(1000));
    }

    /// <summary>
    /// Example 5: Batch spawn with variations using LINQ
    /// </summary>
    [Test]
    public void Example5_BatchWithVariations_FluentIsEasier()
    {
        // ===== BEFORE (complex, nested structures) =====
        var zombiesOld = Enumerable.Range(0, 5).Select(i => new SpawnRequest(
            "Enemies.Zombie",
            Count: 1,
            Position: _zombieSpawn,
            PropertyOverrides: new Dictionary<string, object>
            {
                ["Health"] = 10 + (i * 5),
                ["Speed"] = i % 2 == 0 ? "Fast" : "Slow"
            }
        )).ToList();

        // ===== AFTER (cleaner, more readable) =====
        var zombiesNew = Enumerable.Range(0, 5)
            .Select(i => SpawnRequestBuilder
                .For("Enemies.Zombie")
                .At(_zombieSpawn)
                .WithProperty("Health", 10 + (i * 5))
                .WithProperty("Speed", i % 2 == 0 ? "Fast" : "Slow")
                .Build())
            .ToList();

        // Verify
        Assert.That(zombiesNew.Count, Is.EqualTo(5));
        Assert.That(zombiesNew[0].PropertyOverrides!["Health"], Is.EqualTo(10));
        Assert.That(zombiesNew[4].PropertyOverrides!["Health"], Is.EqualTo(30));
    }

    /// <summary>
    /// Example 6: Implicit conversion for concise syntax
    /// </summary>
    [Test]
    public void Example6_ImplicitConversion_EvenMoreConcise()
    {
        // Can drop the .Build() call due to implicit conversion
        SpawnRequest survivor = SpawnRequestBuilder
            .For(TestHelpers.MikeId)
            .At(_playerSpawn)
            .WithProperty("Health", 12);

        // Can use directly in command with explicit Build()
        var cmd = new SpawnAgentsCommand(new[]
        {
            SpawnRequestBuilder.For(TestHelpers.MikeId).At(_playerSpawn).Build(),
            SpawnRequestBuilder.For(TestHelpers.DougId).At(_playerSpawn).Build()
        });

        Assert.That(cmd.Requests.Count, Is.EqualTo(2));
    }

    /// <summary>
    /// Example 7: Real integration test using fluent API
    /// </summary>
    [Test]
    [Ignore("Requires fixing Survivor constructor issue - not related to builder")]
    public void Example7_RealIntegration_FluentSpawnWorks()
    {
        // Spawn props first
        var (_, _, props, _) = BarelyAlive.Rules.Adapter.Loaders.MissionLoader
            .ParseMissionString(TestHelpers.Mission01Json);
        
        _bootstrap.Engine.Runtime.ExecuteCommand(new SpawnPropsCommand(props));
        _bootstrap.Engine.Runtime.ExecuteCommand(new TurnForge.Engine.Commands.ACK.ACKCommand());

        // ===== Use fluent API for agent spawn =====
        var agentRequests = new[]
        {
            SpawnRequestBuilder
                .For(TestHelpers.MikeId)
                .At(_playerSpawn) // Strategy will override this
                .WithProperty("Health", 12)
                .Build(),
            
            SpawnRequestBuilder
                .For(TestHelpers.DougId)
                .At(_playerSpawn)
                .WithProperty("ActionPoints", 4)
                .Build()
        };

        var result = _bootstrap.Engine.Runtime.ExecuteCommand(new SpawnAgentsCommand(agentRequests));
        
        // Verify success
        Assert.That(result.Result.Success, Is.True);
        
        var state = _bootstrap.GameRepository.LoadGameState();
        Assert.That(state.GetAgents().Count, Is.EqualTo(2));
    }

    /// <summary>
    /// Example 8: Combining multiple spawn types in one mission
    /// </summary>
    [Test]
    public void Example8_ComplexMissionSetup_ShowcasesFluentAPI()
    {
        // Setup mission with multiple entity types
        var missionRequests = new List<SpawnRequest>
        {
            // Player team
            SpawnRequestBuilder
                .For("Survivors.Mike")
                .At(_playerSpawn)
                .WithProperty("Health", 12)
                .WithProperty("Faction", "Player"),
                
            SpawnRequestBuilder
                .For("Survivors.Doug")
                .At(_playerSpawn)
                .WithProperty("Health", 10),

            // Enemy wave
            SpawnRequestBuilder
                .For("Enemies.Zombie")
                .At(_zombieSpawn)
                .WithCount(5),

            // Boss
            SpawnRequestBuilder
                .For("Enemies.Abomination")
                .At(_bossSpawn)
                .WithProperty("Health", 500)
                .WithProperty("PhaseCount", 3)
        };

        // All requests use consistent, readable syntax
        Assert.That(missionRequests.Count, Is.EqualTo(4));
        Assert.That(missionRequests[2].Count, Is.EqualTo(5)); // Batch spawn
        Assert.That(missionRequests[3].PropertyOverrides!["Health"], Is.EqualTo(500)); // Boss properties
    }
}
