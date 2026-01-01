namespace Parchis.Rules.Board;

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
    
    

/// <summary>
/// Player colors in 4-player Parchís.
/// </summary>
public enum PlayerColor
{
    Yellow = 0,
    Blue = 1,
    Red = 2,
    Green = 3
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

}
