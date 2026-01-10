using ParchisLudo.Rules.Board;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;
using static ParchisLudo.Rules.Board.ParchisBoard;

namespace ParchisLudo.Rules.Extensions;

/// <summary>
/// Domain-specific extensions for GameStateView in Parchis.
/// Provides semantic API like GetPawns() instead of generic GetEntity().
/// </summary>
public static class ParchisViewExtensions
{
    // ─────────────────────────────────────────────────────────────────
    // Pawn Queries
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Get all pawns owned by a player.
    /// </summary>
    public static IEnumerable<Actor> GetPawns(this GameStateView view, PlayerId owner)
        => view.GetEntitiesByOwner(owner).OfType<Actor>();

    /// <summary>
    /// Get pawns at a specific tile.
    /// </summary>
    public static IEnumerable<Actor> GetPawnsAt(this GameStateView view, TileId tile)
        => view.GetEntitiesAt(new TilePosition(tile)).OfType<Actor>();

    /// <summary>
    /// Get the first pawn at a tile (or null if empty).
    /// </summary>
    public static Actor? GetPawnAt(this GameStateView view, TileId tile)
        => view.GetPawnsAt(tile).FirstOrDefault();

    /// <summary>
    /// Get pawns of a player that are in their spawn zone.
    /// </summary>
    public static IEnumerable<Agent> GetPawnsInSpawn(this GameStateView view, PlayerId owner)
    {

        var spawnTile = (TilePosition)ParchisBoard.GetSpawnPosition(ParchisBoard.StringToColor(owner));
        var res = view.GetPawnsAt(spawnTile.tileId);
        //filter by type Actor
        return res.OfType<Agent>();
        /*return view.GetPawnsAt(spawnTile.tileId).Where(p =>
        {
            var teamComponent = p.GetComponent<TurnForge.Engine.Components.TeamComponent>();
            return teamComponent?.PlayerId == owner;
        });*/
    }


    /// <summary>
    /// Get the entry tile for a player.
    /// </summary>
    public static TilePosition GetForwardPosition(this GameStateView view, Actor pawn, bool moveBack = false)
    {
        /* TODO: Fix this method - requires ColorTrait definition and GetAdjacentConnectionPositions API
        // check if we are 
        var position = (TilePosition)pawn.CurrentPosition;
        var intTrack = ParchisBoard.TrackToInt(position.TileId.Value);
        if (pawn.TryGetTrait<ColorTrait>(out var colorTrait) || colorTrait == null)
            throw new Exception("Invalid pawn");
        var adjacentConnections = view.GetAdjacentConnectionPositions(position);
        // if adjacents connections == 2 --> we are not in a finsh line start
        if (adjacentConnections.Count() == 2)
        {
            //check if one of the connections is the center
            if (adjacentConnections.Any(c => c.connection.To.Value.ToLower().StartsWith("center")))
            {
                return (TilePosition)ParchisBoard.CenterPosition;
            }
            var connectedIds = adjacentConnections.Select(c => ParchisBoard.TrackToInt(c.connection.To.Value)).ToList();
            // check if we are in finnsh line
            if (position.TileId.Value.StartsWith("finish_"))
            {
                return GetForwardPositionInFinishLane(intTrack, connectedIds, colorTrait.Color, moveBack);
            }
            else
            {
                return GetForwardPositionInTrack(intTrack, connectedIds, moveBack);
            }
        }
        else
        {
            // we are in the starting tile of the finish lane, need to check if it's our color
            var finishAdjacentConnections = adjacentConnections.FirstOrDefault(c => c.connection.To.Value.StartsWith("finish_"));
            if (finishAdjacentConnections.ConnectionProps.Length > 0)
                throw new Exception("Invalid finish lane");

            if (!finishAdjacentConnections.ConnectionProps[0].TryGetTrait<ColorTrait>(out var colorTraitLine) || colorTraitLine == null)
                throw new Exception("Invalid Finsh line connection");
            if (colorTraitLine.Color != colorTrait.Color)
            {
                // the pawn continuw in the track
                var filteredAdjacentConnections = adjacentConnections.Where(c => c.connection.To.Value.StartsWith("track_"));
                var connectedIds = filteredAdjacentConnections.Select(c => ParchisBoard.TrackToInt(c.connection.To.Value)).ToList();
                return GetForwardPositionInTrack(intTrack, connectedIds, moveBack);
            }
            return new TilePosition(new TileId(finishAdjacentConnections.connection.To.Value));
        }
        */
        throw new NotImplementedException("GetForwardPosition requires ColorTrait and GetAdjacentConnectionPositions");
    }

    /* TODO: Fix these helper methods - require ColorTrait definition
    private static TilePosition GetForwardPositionInFinishLane(int intTrack, List<int> connectedIds, PlayerColor color, bool moveBack)
    {
        // check if one of the connection is the center

        if (connectedIds[0] > intTrack)
        {
            return moveBack ? new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[1], color)))
                            : new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[0], color)));
        }
        return moveBack ? new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[0], color)))
                        : new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[1], color)));
    }

    private static TilePosition GetForwardPositionInTrack(int intTrack, List<int> connectedIds, bool moveBack)
    {
        // check if one of the connection is the center
        if (connectedIds[0] > intTrack || connectedIds[0] == 1)
        {
            return moveBack ? new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[1])))
                            : new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[0])));
        }
        return moveBack ? new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[0])))
                        : new TilePosition(new TileId(ParchisBoard.IntToTrack(connectedIds[1])));
    }
    */


    /// <summary>
    /// Get pawns of a player that are on the track (not in spawn or finish).
    /// </summary>
    public static IEnumerable<Actor> GetPawnsOnTrack(this GameStateView view, PlayerId owner)
    {
        var res = view.GetPawns(owner).Where(p =>
        {
            var pos = view.GetPosition(p.Id);
            if (pos is TilePosition tp)
            {
                return tp.TileId.Value.StartsWith("track_");
            }
            return false;
        });
        return res.OfType<Agent>();
    }

    /// <summary>
    /// Get pawns of a player that are in their finish lane.
    /// </summary>
    public static IEnumerable<Actor> GetPawnsInFinishLane(this GameStateView view, PlayerId owner, string color)
    {
        return view.GetPawns(owner).Where(p =>
        {
            var pos = view.GetPosition(p.Id);
            if (pos is TilePosition tp)
            {
                return tp.TileId.Value.StartsWith($"{color}_finish_");
            }
            return false;
        });
    }


    // ─────────────────────────────────────────────────────────────────
    // Position Queries
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Get the current tile of a pawn.
    /// </summary>
    public static TileId? GetPawnTile(this GameStateView view, Actor pawn)
    {
        var pos = view.GetPosition(pawn.Id);
        return pos is TilePosition tp ? tp.TileId : null;
    }

    /// <summary>
    /// Get the current tile of a pawn by ID.
    /// </summary>
    public static TileId? GetPawnTile(this GameStateView view, EntityId pawnId)
    {
        var pos = view.GetPosition(pawnId);
        return pos is TilePosition tp ? tp.TileId : null;
    }
}
