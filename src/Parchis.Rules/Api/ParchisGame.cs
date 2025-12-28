using Parchis.Rules.Board;
using Parchis.Rules.Definitions;
using TurnForge.Engine.Definitions;

namespace Parchis.Rules.Api;

/// <summary>
/// Main API for the Parchís game.
/// Provides high-level commands for UI to interact with the game.
/// </summary>
public class ParchisGame
{
    private GameState _state;
    private readonly Dictionary<string, PieceInfo> _pieces = new();
    
    public ParchisGame()
    {
        _state = GameState.Empty();
    }
    
    /// <summary>
    /// Initialize the game: create board.
    /// </summary>
    public InitGameResult InitGame()
    {
        // Create board
        _state = ParchisBootstrap.CreateInitialStateWithBoard();
        
        return new InitGameResult
        {
            Success = true,
            BoardTiles = ParchisBoard.MainCircuitSize + (ParchisBoard.FinishLaneSize * 2) + 8 + 1,
            SafeZones = new[] { 0, 12, 17, 29, 34, 46, 51, 63 }
        };
    }
    
    /// <summary>
    /// Start the game: spawn all pieces.
    /// </summary>
    public StartGameResult StartGame()
    {
        _pieces.Clear();
        
        foreach (PlayerColor color in Enum.GetValues<PlayerColor>())
        {
            for (int i = 0; i < ParchisBootstrap.PiecesPerPlayer; i++)
            {
                var id = $"{color}_{i}";
                _pieces[id] = new PieceInfo
                {
                    Color = color,
                    PieceNumber = i,
                    Location = PieceLocation.Home,
                    Position = i
                };
            }
        }
        
        return new StartGameResult
        {
            Success = true,
            CurrentPlayer = PlayerColor.Yellow,
            Pieces = _pieces.Values.ToArray()
        };
    }
    
    /// <summary>
    /// Roll the dice for current player.
    /// </summary>
    public RollDiceResult RollDice()
    {
        var random = new Random();
        var die1 = random.Next(1, 7);
        var die2 = random.Next(1, 7);
        
        var currentPlayer = (PlayerColor)_state.Metadata["CurrentPlayer"];
        var consecutiveSixes = (int)_state.Metadata["ConsecutiveSixes"];
        
        bool isDouble = die1 == die2;
        bool isThreeSixes = isDouble && die1 == 6 && consecutiveSixes >= 2;
        
        // Update consecutive sixes count
        if (isDouble && die1 == 6)
        {
            consecutiveSixes++;
            _state = _state.WithMetadata("ConsecutiveSixes", consecutiveSixes);
        }
        else
        {
            _state = _state.WithMetadata("ConsecutiveSixes", 0);
        }
        
        // Calculate valid moves
        var validMoves = CalculateValidMoves(currentPlayer, die1 + die2);
        
        return new RollDiceResult
        {
            Success = true,
            Die1 = die1,
            Die2 = die2,
            Total = die1 + die2,
            IsDouble = isDouble,
            ExtraRoll = isDouble && !isThreeSixes,
            PenaltyThreeSixes = isThreeSixes,
            ValidMoves = validMoves
        };
    }
    
    /// <summary>
    /// Move a piece.
    /// </summary>
    public MovePieceResult MovePiece(PlayerColor color, int pieceNumber, int steps)
    {
        var id = $"{color}_{pieceNumber}";
        if (!_pieces.TryGetValue(id, out var piece))
        {
            return new MovePieceResult { Success = false, Error = "Piece not found" };
        }
        
        // Handle leaving home (requires 5)
        if (piece.Location == PieceLocation.Home)
        {
            if (steps != 5)
            {
                return new MovePieceResult { Success = false, Error = "Need 5 to leave home" };
            }
            
            var entryPoint = color == PlayerColor.Yellow ? ParchisBoard.YellowEntry : ParchisBoard.BlueEntry;
            _pieces[id] = piece with { Location = PieceLocation.Track, Position = entryPoint };
            
            return new MovePieceResult
            {
                Success = true,
                NewLocation = PieceLocation.Track,
                NewPosition = entryPoint,
                CapturedEnemy = CheckCapture(color, entryPoint)
            };
        }
        
        // Handle track movement
        if (piece.Location == PieceLocation.Track)
        {
            var newPosition = ParchisBoard.GetNextPosition(piece.Position, steps);
            
            // Check if should enter finish lane
            var finishEntry = color == PlayerColor.Yellow ? ParchisBoard.YellowFinishEntry : ParchisBoard.BlueFinishEntry;
            bool entersFinish = piece.Position <= finishEntry && (piece.Position + steps) > finishEntry;
            
            if (entersFinish)
            {
                var finishPosition = steps - (finishEntry - piece.Position) - 1;
                if (finishPosition >= ParchisBoard.FinishLaneSize)
                {
                    return new MovePieceResult { Success = false, Error = "Overshot finish lane" };
                }
                
                _pieces[id] = piece with { Location = PieceLocation.FinishLane, Position = finishPosition };
                
                return new MovePieceResult
                {
                    Success = true,
                    NewLocation = PieceLocation.FinishLane,
                    NewPosition = finishPosition,
                    EnteredFinishLane = true
                };
            }
            
            _pieces[id] = piece with { Position = newPosition };
            
            return new MovePieceResult
            {
                Success = true,
                NewLocation = PieceLocation.Track,
                NewPosition = newPosition,
                CapturedEnemy = CheckCapture(color, newPosition)
            };
        }
        
        // Handle finish lane movement
        if (piece.Location == PieceLocation.FinishLane)
        {
            var newPosition = piece.Position + steps;
            
            if (newPosition == ParchisBoard.FinishLaneSize)
            {
                _pieces[id] = piece with { Location = PieceLocation.Finished, Position = 0 };
                
                return new MovePieceResult
                {
                    Success = true,
                    NewLocation = PieceLocation.Finished,
                    NewPosition = 0,
                    Finished = true
                };
            }
            
            if (newPosition > ParchisBoard.FinishLaneSize)
            {
                return new MovePieceResult { Success = false, Error = "Overshot goal" };
            }
            
            _pieces[id] = piece with { Position = newPosition };
            
            return new MovePieceResult
            {
                Success = true,
                NewLocation = PieceLocation.FinishLane,
                NewPosition = newPosition
            };
        }
        
        return new MovePieceResult { Success = false, Error = "Piece already finished" };
    }
    
    /// <summary>
    /// End turn and switch to next player.
    /// </summary>
    public EndTurnResult EndTurn()
    {
        var currentPlayer = (PlayerColor)_state.Metadata["CurrentPlayer"];
        var nextPlayer = currentPlayer == PlayerColor.Yellow ? PlayerColor.Blue : PlayerColor.Yellow;
        var turnNumber = (int)_state.Metadata["TurnNumber"];
        
        _state = _state
            .WithMetadata("CurrentPlayer", nextPlayer)
            .WithMetadata("TurnNumber", turnNumber + 1)
            .WithMetadata("ConsecutiveSixes", 0);
        
        return new EndTurnResult
        {
            Success = true,
            NextPlayer = nextPlayer,
            TurnNumber = turnNumber + 1
        };
    }
    
    /// <summary>
    /// Get current game state for UI.
    /// </summary>
    public GameStateSnapshot GetSnapshot()
    {
        return new GameStateSnapshot
        {
            CurrentPlayer = (PlayerColor)_state.Metadata["CurrentPlayer"],
            TurnNumber = (int)_state.Metadata["TurnNumber"],
            Pieces = _pieces.Values.ToArray()
        };
    }
    
    private ValidMove[] CalculateValidMoves(PlayerColor color, int total)
    {
        var moves = new List<ValidMove>();
        
        foreach (var piece in _pieces.Values.Where(p => p.Color == color))
        {
            if (piece.Location == PieceLocation.Home && total == 5)
            {
                var entry = color == PlayerColor.Yellow ? ParchisBoard.YellowEntry : ParchisBoard.BlueEntry;
                moves.Add(new ValidMove
                {
                    PieceNumber = piece.PieceNumber,
                    TargetLocation = PieceLocation.Track,
                    TargetPosition = entry,
                    WouldCapture = WouldCapture(color, entry)
                });
            }
            else if (piece.Location == PieceLocation.Track)
            {
                var newPos = ParchisBoard.GetNextPosition(piece.Position, total);
                moves.Add(new ValidMove
                {
                    PieceNumber = piece.PieceNumber,
                    TargetLocation = PieceLocation.Track,
                    TargetPosition = newPos,
                    WouldCapture = WouldCapture(color, newPos)
                });
            }
            else if (piece.Location == PieceLocation.FinishLane)
            {
                var newPos = piece.Position + total;
                if (newPos <= ParchisBoard.FinishLaneSize)
                {
                    moves.Add(new ValidMove
                    {
                        PieceNumber = piece.PieceNumber,
                        TargetLocation = newPos == ParchisBoard.FinishLaneSize ? PieceLocation.Finished : PieceLocation.FinishLane,
                        TargetPosition = newPos == ParchisBoard.FinishLaneSize ? 0 : newPos,
                        WouldFinish = newPos == ParchisBoard.FinishLaneSize
                    });
                }
            }
        }
        
        return moves.ToArray();
    }
    
    private bool WouldCapture(PlayerColor color, int position)
    {
        if (ParchisBoard.IsSafeZone(position)) return false;
        return _pieces.Values.Any(p => p.Color != color && p.Location == PieceLocation.Track && p.Position == position);
    }
    
    private bool CheckCapture(PlayerColor color, int position)
    {
        if (ParchisBoard.IsSafeZone(position)) return false;
        
        var captured = _pieces.Values
            .Where(p => p.Color != color && p.Location == PieceLocation.Track && p.Position == position)
            .ToList();
        
        foreach (var enemy in captured)
        {
            var id = $"{enemy.Color}_{enemy.PieceNumber}";
            _pieces[id] = enemy with { Location = PieceLocation.Home, Position = enemy.PieceNumber };
        }
        
        return captured.Any();
    }
}

// ============================================================================
// Result DTOs for UI
// ============================================================================

public record InitGameResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public int BoardTiles { get; init; }
    public int[] SafeZones { get; init; } = Array.Empty<int>();
}

public record StartGameResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public PlayerColor CurrentPlayer { get; init; }
    public PieceInfo[] Pieces { get; init; } = Array.Empty<PieceInfo>();
}

public record RollDiceResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public int Die1 { get; init; }
    public int Die2 { get; init; }
    public int Total { get; init; }
    public bool IsDouble { get; init; }
    public bool ExtraRoll { get; init; }
    public bool PenaltyThreeSixes { get; init; }
    public ValidMove[] ValidMoves { get; init; } = Array.Empty<ValidMove>();
}

public record MovePieceResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public PieceLocation NewLocation { get; init; }
    public int NewPosition { get; init; }
    public bool CapturedEnemy { get; init; }
    public bool EnteredFinishLane { get; init; }
    public bool Finished { get; init; }
}

public record EndTurnResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public PlayerColor NextPlayer { get; init; }
    public int TurnNumber { get; init; }
}

public record GameStateSnapshot
{
    public PlayerColor CurrentPlayer { get; init; }
    public int TurnNumber { get; init; }
    public PieceInfo[] Pieces { get; init; } = Array.Empty<PieceInfo>();
}

public record PieceInfo
{
    public PlayerColor Color { get; init; }
    public int PieceNumber { get; init; }
    public PieceLocation Location { get; init; }
    public int Position { get; init; }
}

public record ValidMove
{
    public int PieceNumber { get; init; }
    public PieceLocation TargetLocation { get; init; }
    public int TargetPosition { get; init; }
    public bool WouldCapture { get; init; }
    public bool WouldFinish { get; init; }
}
