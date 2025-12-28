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
    
    // Entry points for each player
    public static readonly int YellowEntry = 0;
    public static readonly int BlueEntry = 34;
    
    // Finish lane size (per player)
    public const int FinishLaneSize = 8;
    
    // Safe zones (no capture allowed)
    private static readonly HashSet<int> SafeZones = new()
    {
        0, 12, 17, 29, 34, 46, 51, 63
    };
    
    // Entry tiles for finish lanes (one step before entering)
    // Yellow enters finish lane after position 67
    // Blue enters finish lane after position 33
    public static readonly int YellowFinishEntry = 67;
    public static readonly int BlueFinishEntry = 33;
    
    /// <summary>
    /// Check if a position is a safe zone (no captures).
    /// </summary>
    public static bool IsSafeZone(int position)
    {
        return SafeZones.Contains(position);
    }
    
    /// <summary>
    /// Get the next position on the main circuit.
    /// </summary>
    public static int GetNextPosition(int current, int steps)
    {
        return (current + steps) % MainCircuitSize;
    }
    
    /// <summary>
    /// Check if a piece should enter the finish lane.
    /// </summary>
    public static bool ShouldEnterFinishLane(PlayerColor color, int currentPosition, int steps)
    {
        var finishEntry = color == PlayerColor.Yellow ? YellowFinishEntry : BlueFinishEntry;
        var entryPoint = color == PlayerColor.Yellow ? YellowEntry : BlueEntry;
        
        // Calculate if we pass the finish entry point
        var targetPosition = GetNextPosition(currentPosition, steps);
        
        // Check if we've lapped around to our finish lane entry
        // This requires checking if we cross from before entry to after
        return CrossesFinishEntry(currentPosition, steps, finishEntry, entryPoint);
    }
    
    private static bool CrossesFinishEntry(int current, int steps, int finishEntry, int entryPoint)
    {
        // Simplified check: if we're at or past finish entry on our lap
        for (int i = 1; i <= steps; i++)
        {
            if (GetNextPosition(current, i) == (finishEntry + 1) % MainCircuitSize)
            {
                // Check if this is our second lap (after passing entry point)
                // For simplicity, we track this externally via piece state
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// Player colors in 2-player Parchís.
/// </summary>
public enum PlayerColor
{
    Yellow = 0,
    Blue = 1
}

/// <summary>
/// Represents a piece's current location state.
/// </summary>
public enum PieceLocation
{
    /// <summary>Piece is in the home base, not yet in play.</summary>
    Home,
    
    /// <summary>Piece is on the main circuit track.</summary>
    Track,
    
    /// <summary>Piece is in the finish lane.</summary>
    FinishLane,
    
    /// <summary>Piece has reached the center and finished.</summary>
    Finished
}

/// <summary>
/// Represents a position on the board.
/// </summary>
public readonly record struct BoardPosition
{
    /// <summary>Location type (Home, Track, FinishLane, Finished).</summary>
    public PieceLocation Location { get; init; }
    
    /// <summary>
    /// Position index. Meaning depends on Location:
    /// - Home: piece number (0-3)
    /// - Track: circuit position (0-67)
    /// - FinishLane: lane position (0-7)
    /// - Finished: 0
    /// </summary>
    public int Index { get; init; }
    
    public static BoardPosition AtHome(int pieceNumber) => new() { Location = PieceLocation.Home, Index = pieceNumber };
    public static BoardPosition OnTrack(int position) => new() { Location = PieceLocation.Track, Index = position };
    public static BoardPosition InFinishLane(int position) => new() { Location = PieceLocation.FinishLane, Index = position };
    public static BoardPosition Finished => new() { Location = PieceLocation.Finished, Index = 0 };
}
