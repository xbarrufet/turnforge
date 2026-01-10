using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Rules.Extensions;

/// <summary>
/// Parchis-specific query extensions for GameState.
/// </summary>
public static class ParchisQueryExtensions
{
  /*  /// <summary>
    /// Get pawns NOT in spawn (on track or finish).
    /// </summary>
    public static GameStateQuery NotInSpawn(this GameStateQuery query)
    {
        return query.AtLocation(pos =>
            pos is TilePosition tp && !tp.TileId.Value.Contains("spawn"));
    }



    /// <summary>
    /// Get pawns on track.
    /// </summary>
    public static GameStateQuery OnTrack(this GameStateQuery query)
    {
        return query.AtLocation(pos =>
            pos is TilePosition tp && tp.TileId.Value.StartsWith("track_"));
    }

    /// <summary>
    /// Get pawns in finish lane for a specific color.
    /// </summary>
    public static GameStateQuery InFinishLane(this GameStateQuery query, string color)
    {
        return query.AtLocation(pos =>
            pos is TilePosition tp && tp.TileId.Value.StartsWith($"{color}_finish_"));
    }

    /// <summary>
    /// Get pawns in spawn.
    /// </summary>
    public static GameStateQuery InSpawn(this GameStateQuery query)
    {
        return query.AtLocation(pos =>
            pos is TilePosition tp && tp.TileId.Value.Contains("spawn"));
    }

    /// <summary>
    /// Get pawns in home (final position).
    /// </summary>
    public static GameStateQuery InHome(this GameStateQuery query, string color)
    {
        return query.AtLocation(pos =>
            pos is TilePosition tp && tp.TileId.Value == $"{color}_home");
    }*/
}
