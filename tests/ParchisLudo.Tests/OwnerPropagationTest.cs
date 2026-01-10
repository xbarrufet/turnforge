using NUnit.Framework;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Tests;

[TestFixture]
public class OwnerPropagationTest
{
  /*  [Test]
    public void TeamComponent_OwnerId_PersistsAfterOverlayCommit()
    {
        // Arrange: Create a player and a pawn with OwnerId set
        var playerId = PlayerId.From("red");
        var pawnId = EntityId.New();

        // Create pawn with TeamComponent that has OwnerId
        var pawn = new Agent(pawnId, "pawn_red_0", "Red Pawn 0", "Pawn");
        var teamTrait = new MembershipTrait("red", "Player", playerId);
        var teamComponent = new TeamComponent(teamTrait);

        pawn.ReplaceComponent(teamComponent);

        // Verify OwnerId is set initially
        var initialTeamComp = pawn.GetComponent<ITeamComponent>() as TeamComponent;
        Assert.That(initialTeamComp, Is.Not.Null);
        Assert.That(initialTeamComp!.PlayerId, Is.EqualTo(playerId), "OwnerId should be set initially");

        // Act: Spawn entity through overlay and commit
        var baseState = GameState.Empty();
        var spawnOp = new SpawnEntityOperation(pawn, new TilePosition(new TileId("spawn_red")));

        baseState.RecordOverlayOperation(spawnOp);
        var committedState = baseState.CommitOverlayChanges();

        // Assert: Verify OwnerId persists after commit
        var retrievedPawn = committedState.GetOverlayedEntity(pawnId);
        var committedTeamComp = retrievedPawn.GetComponent<ITeamComponent>() as TeamComponent;

        Assert.That(committedTeamComp, Is.Not.Null, "TeamComponent should exist after commit");
        Assert.That(committedTeamComp!.PlayerId, Is.EqualTo(playerId), "OwnerId should persist after overlay commit");
    }

    [Test]
    public void GetEntitiesByOwner_ReturnsCorrectPawns_AfterCommit()
    {
        // Arrange: Create multiple players and pawns
        var redPlayer = PlayerId.From("red");
        var bluePlayer = PlayerId.From("blue");

        var redPawn1 = new Agent(EntityId.New(), "pawn_red_0", "Red Pawn 0", "Pawn");
        redPawn1.ReplaceComponent(new TeamComponent(new MembershipTrait("red", "Player", redPlayer)));

        var redPawn2 = new Agent(EntityId.New(), "pawn_red_1", "Red Pawn 1", "Pawn");
        redPawn2.ReplaceComponent(new TeamComponent(new MembershipTrait("red", "Player", redPlayer)));

        var bluePawn1 = new Agent(EntityId.New(), "pawn_blue_0", "Blue Pawn 0", "Pawn");
        bluePawn1.ReplaceComponent(new TeamComponent(new MembershipTrait("blue", "Player", bluePlayer)));

        // Act: Spawn entities and commit
        var baseState = GameState.Empty();
        baseState.RecordOverlayOperation(new SpawnEntityOperation(redPawn1, new TilePosition(new TileId("spawn_red"))));
        baseState.RecordOverlayOperation(new SpawnEntityOperation(redPawn2, new TilePosition(new TileId("spawn_red"))));
        baseState.RecordOverlayOperation(new SpawnEntityOperation(bluePawn1, new TilePosition(new TileId("spawn_blue"))));

        var committedState = baseState.CommitOverlayChanges();
        var stateView = new GameStateView(committedState);

        // Assert: GetEntitiesByOwner returns correct pawns
        var redPawns = stateView.GetEntitiesByOwner(redPlayer).ToList();
        var bluePawns = stateView.GetEntitiesByOwner(bluePlayer).ToList();

        Assert.That(redPawns, Has.Count.EqualTo(2), "Should return 2 red pawns");
        Assert.That(bluePawns, Has.Count.EqualTo(1), "Should return 1 blue pawn");

        Assert.That(redPawns.Select(p => p.Id), Does.Contain(redPawn1.Id));
        Assert.That(redPawns.Select(p => p.Id), Does.Contain(redPawn2.Id));
        Assert.That(bluePawns.Select(p => p.Id), Does.Contain(bluePawn1.Id));
    }*/
}
