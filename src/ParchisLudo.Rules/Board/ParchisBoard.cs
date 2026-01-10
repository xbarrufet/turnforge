using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace ParchisLudo.Rules.Board;

/// <summary>
/// Represents the Parchís game board topology.
/// 
/// Board layout:
/// - 68 main circuit tiles (0-67)
/// - Each player has a home zone and finish lane
/// - Players: Yellow (enters at 0), Blue (enters at 34)
/// </summary>
public class ParchisBoard
{
    // Main circuit size
    public const int MainCircuitSize = 68;
    public const int FinishLaneSize = 8;


    // Entry points for each player
    public static readonly string YellowEntry = "track_5";
    public static readonly string BlueEntry = "track_22";
    public static readonly string RedEntry = "track_39";
    public static readonly string GreenEntry = "track_56";

    public static TilePosition GetEntryForColor(PlayerColor color)
    {
        return color switch
        {
            PlayerColor.Yellow => new TilePosition(new TileId(YellowEntry)),
            PlayerColor.Blue => new TilePosition(new TileId(BlueEntry)),
            PlayerColor.Red => new TilePosition(new TileId(RedEntry)),
            PlayerColor.Green => new TilePosition(new TileId(GreenEntry)),
            _ => throw new ArgumentException("Invalid player color")
        };
    }


    // Safe zones (no capture allowed)
    private static readonly HashSet<string> SafeZones = new()
    {
         "track_12", "track_17", "track_29", "track_34", "track_46", "track_51", "track_63", "track_68"
    };

    // Entry tiles for finish lanes (one step before entering)
    // Yellow enters finish lane after position 67
    // Blue enters finish lane after position 33
    public static readonly string YellowFinishEntry = "track_67";
    public static readonly string BlueFinishEntry = "track_33";
    public static readonly string RedFinishEntry = "track_50";
    public static readonly string GreenFinishEntry = "track_67";


    public static readonly string YellowSpawn = "yellow_spawn";
    public static readonly string BlueSpawn = "blue_spawn";
    public static readonly string RedSpawn = "red_spawn";
    public static readonly string GreenSpawn = "green_spawn";
    
    public static string ColorToString(PlayerColor color)
    {
        return color switch
        {
            PlayerColor.Yellow => "yellow",
            PlayerColor.Blue => "blue",
            PlayerColor.Red => "red",
            PlayerColor.Green => "green",
            _ => "undefined"
        };
    }
    
    public TileId GetSpawnTileId(PlayerColor color)
    {
        return color switch
        {
            PlayerColor.Yellow => new TileId(YellowSpawn),
            PlayerColor.Blue => new TileId(BlueSpawn),
            PlayerColor.Red => new TileId(RedSpawn),
            PlayerColor.Green => new TileId(GreenSpawn),
            _ => throw new ArgumentException("Invalid player color")
        };
    }

    public static readonly string Center = "center";

    public static IBoardPosition GetSpawnPosition(PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.Yellow:
                return YellowSpawnPosition;
            case PlayerColor.Blue:
                return BlueSpawnPosition;
            case PlayerColor.Red:
                return RedSpawnPosition;
            case PlayerColor.Green:
                return GreenSpawnPosition;
            default:
                throw new ArgumentException("Invalid player color");
        }
    }

    public static IBoardPosition CenterPosition => new TilePosition(new TileId(Center));
    public static IBoardPosition YellowSpawnPosition => new TilePosition(new TileId(YellowSpawn));
    public static IBoardPosition BlueSpawnPosition => new TilePosition(new TileId(BlueSpawn));
    public static IBoardPosition RedSpawnPosition => new TilePosition(new TileId(RedSpawn));
    public static IBoardPosition GreenSpawnPosition => new TilePosition(new TileId(GreenSpawn));



    /// <summary>
    /// Player colors in 4-player Parchís.
    /// </summary>
    public enum PlayerColor
    {
        Yellow = 0,
        Blue = 1,
        Red = 2,
        Green = 3,
        UNDEFINED = 10
    }
    
    public static PlayerColor StringToColor(string color)
    {
        return color.ToLower() switch
        {
            "yellow" => PlayerColor.Yellow,
            "blue" => PlayerColor.Blue,
            "red" => PlayerColor.Red,
            "green" => PlayerColor.Green,
            _ => PlayerColor.UNDEFINED
        };
    }

    /// <summary>
    /// Possible locations for a game piece.
    /// </summary>
    public enum PieceLocation
    {
        Home,
        Track,
        FinishLane,
        Finished
    }


    public static int TrackToInt(string track)
    {
        if (isFinishLane(track))
        {
            // syntax {color}_finish_{track}
            return int.Parse(track.Split('_')[2]);
        }
        return int.Parse(track.Split('_')[1]);
    }
    public static string IntToTrack(int track, PlayerColor? color = null)
    {
        if (color.HasValue)
        {
            return $"{color.Value}_finish_{track}";
        }
        return $"track_{track}";
    }

    public static bool isFinishLane(string tile)
    {
        return tile.StartsWith("finish_");
    }




}
