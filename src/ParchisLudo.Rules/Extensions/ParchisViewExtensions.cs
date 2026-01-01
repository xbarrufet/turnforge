using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Actors; // ADDED
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Board;

namespace Parchis.Rules.Extensions;

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
        => view.GetEntitiesForOwner(owner).OfType<Actor>();
    
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
    public static IEnumerable<Actor> GetPawnsInSpawn(this GameStateView view, PlayerId owner, string color)
    {
        var spawnTile = new TileId($"spawn_{color}");
        return view.GetPawnsAt(spawnTile).Where(p => 
        {
            var teamComponent = p.GetComponent<TurnForge.Engine.Components.TeamComponent>();
            return teamComponent?.OwnerId == owner;
        });
    }
    
    /// <summary>
    /// Get pawns of a player that are on the track (not in spawn or finish).
    /// </summary>
    public static IEnumerable<Actor> GetPawnsOnTrack(this GameStateView view, PlayerId owner)
    {
        return view.GetPawns(owner).Where(p =>
        {
            var pos = view.GetPosition(p.Id);
            if (pos is TilePosition tp)
            {
                var tileId = tp.TileId.Value;
                return tileId.StartsWith("track_");
            }
            return false;
        });
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
    // Tile Queries
    // ─────────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Check if a tile is occupied by any pawn.
    /// </summary>
    public static bool IsTileOccupied(this GameStateView view, TileId tile)
        => view.GetPawnAt(tile) != null;
    
    /// <summary>
    /// Check if a tile is a safe tile (cannot be captured).
    /// Safe tiles in Parchis are: entry tiles and center.
    /// </summary>
    public static bool IsSafeTile(this GameStateView view, TileId tile)
    {
        var id = tile.Value;
        // Entry tiles and center are safe
        return id == ParchisBoard.RedEntry || 
               id == ParchisBoard.BlueEntry || 
               id == ParchisBoard.GreenEntry || 
               id == ParchisBoard.YellowEntry ||
               id == "center";
    }
    
    /// <summary>
    /// Get the opponent pawn at a tile (for capture logic).
    /// </summary>
    public static Actor? GetOpponentPawnAt(this GameStateView view, TileId tile, string myColor)
    {
        return view.GetPawnsAt(tile).FirstOrDefault(p =>
        {
            var team = p.GetComponent<TurnForge.Engine.Components.Interfaces.ITeamComponent>()?.Team;
            return team != null && !team.Equals(myColor, StringComparison.OrdinalIgnoreCase);
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
