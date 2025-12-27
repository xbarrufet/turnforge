using NUnit.Framework;
using BarelyAlive.Rules.Tests.Infrastructure;
using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Traits.Standard;
using System.Linq;

namespace BarelyAlive.Rules.Tests.Integration;

[TestFixture]
public class SurvivorTraceTests
{
    private TestBootstrap _bootstrap = null!;
    private Position _playerSpawn;

    [SetUp]
    public void Setup()
    {
        _bootstrap = TestBootstrap.CreateNewGame(enableFsm: false);
        
        // Setup simple board
        var (spatial, zones, _, _) = BarelyAlive.Rules.Adapter.Loaders.MissionLoader.ParseMissionString(TestHelpers.Mission01Json);
        var boardDesc = new TurnForge.Engine.Definitions.Board.Descriptors.BoardDescriptor(spatial, zones);
        _bootstrap.Engine.Runtime.ExecuteCommand(new InitializeBoardCommand(boardDesc));
        _bootstrap.Engine.Runtime.ExecuteCommand(new TurnForge.Engine.Commands.ACK.ACKCommand());
        
        _playerSpawn = new Position(new TileId(System.Guid.Parse("07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53")));
    }

    [Test]
    public void Spawn_WithActionPointsTrait_ShouldOverrideDefault()
    {
        // Arrange
        // Default survivor usually gets 3 AP (via strategy fallback).
        // We request 5 AP via Trait.
        var request = SpawnRequestBuilder
            .For(TestHelpers.MikeId)
            .At(_playerSpawn)
            .WithTrait(new ActionPointsTrait(5, 1))
            .Build();

        // Act
        _bootstrap.Engine.Runtime.ExecuteCommand(new SpawnAgentsCommand(new[] { request }));
        _bootstrap.Engine.Runtime.ExecuteCommand(new TurnForge.Engine.Commands.ACK.ACKCommand());

        // Assert
        var state = _bootstrap.GameRepository.LoadGameState();
        var survivor = state.GetAgents().First(e => e.DefinitionId == TestHelpers.MikeId);
        var apComponent = survivor.GetComponent<IActionPointsComponent>();

        Assert.That(apComponent, Is.Not.Null);
        Assert.That(apComponent.MaxActionPoints, Is.EqualTo(5));
        Assert.That(apComponent.CurrentActionPoints, Is.EqualTo(5));
    }

    [Test]
    public void Spawn_WithTeamTrait_ShouldOverrideDefault()
    {
        // Arrange
        var request = SpawnRequestBuilder
            .For(TestHelpers.MikeId)
            .At(_playerSpawn)
            .WithTrait(new TeamTrait("Rebels", "Rebels"))
            .Build();

        // Act
        _bootstrap.Engine.Runtime.ExecuteCommand(new SpawnAgentsCommand(new[] { request }));
        _bootstrap.Engine.Runtime.ExecuteCommand(new TurnForge.Engine.Commands.ACK.ACKCommand());

        // Assert
        var state = _bootstrap.GameRepository.LoadGameState();
        var survivor = state.GetAgents().First(e => e.DefinitionId == TestHelpers.MikeId);
        
        // Verify TeamTrait is present on entity (via TraitContainer)
        // Since TeamTrait translates to TeamComponent or just sets .Team property? 
        // Let's check how TeamTrait is handled. 
        // GenericActorFactory -> InitializeTraits -> adds to TraitContainer.
        // TraitInitializationService -> wires to TeamComponent?

        Assert.That(survivor.Team, Is.EqualTo("Rebels"));
    }
}
