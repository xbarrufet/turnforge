using Parchis.Rules.Api;
using Parchis.Rules.Board;

namespace Parchis.Simulation;

/// <summary>
/// Console simulation of a Parchís game.
/// Runs automatically until there's a winner.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("                    PARCHÍS SIMULATION                          ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine();
        
        var game = new ParchisGame();
        
        // Initialize game
        var initResult = game.InitGame();
        Console.WriteLine($"🎲 Game initialized: {initResult.BoardTiles} tiles, " +
                          $"Safe zones: [{string.Join(", ", initResult.SafeZones)}]");
        Console.WriteLine();
        
        // Start game
        var startResult = game.StartGame();
        Console.WriteLine($"🎮 Game started! First player: {startResult.CurrentPlayer}");
        PrintPieces(startResult.Pieces);
        Console.WriteLine();
        
        // Game loop
        int maxTurns = 500; // Safety limit
        int turn = 0;
        PlayerColor? winner = null;
        
        while (winner == null && turn < maxTurns)
        {
            turn++;
            var snapshot = game.GetSnapshot();
            
            Console.WriteLine($"══════════════════════════════════════════════════════════════");
            Console.WriteLine($"  TURN {turn} - {snapshot.CurrentPlayer}'s turn");
            Console.WriteLine($"══════════════════════════════════════════════════════════════");
            
            // Roll dice
            var rollResult = game.RollDice();
            Console.WriteLine($"  🎲 Rolled: [{rollResult.Die1}] + [{rollResult.Die2}] = {rollResult.Total}");
            
            if (rollResult.IsDouble)
            {
                Console.WriteLine($"  ⚡ DOUBLE! Extra roll granted.");
            }
            
            if (rollResult.PenaltyThreeSixes)
            {
                Console.WriteLine($"  ⚠️ THREE SIXES IN A ROW! Piece goes home.");
                // TODO: Send most advanced piece home
                game.EndTurn();
                continue;
            }
            
            if (rollResult.ValidMoves.Length == 0)
            {
                Console.WriteLine($"  ❌ No valid moves. Turn skipped.");
                game.EndTurn();
                continue;
            }
            
            // Show valid moves
            Console.WriteLine($"  Valid moves:");
            foreach (var move in rollResult.ValidMoves)
            {
                var flags = new List<string>();
                if (move.WouldCapture) flags.Add("CAPTURE");
                if (move.WouldFinish) flags.Add("FINISH");
                var flagStr = flags.Count > 0 ? $" [{string.Join(", ", flags)}]" : "";
                
                Console.WriteLine($"    - Piece {move.PieceNumber}: {move.TargetLocation} pos {move.TargetPosition}{flagStr}");
            }
            
            // AI: Choose the best move
            var chosenMove = ChooseBestMove(rollResult.ValidMoves);
            Console.WriteLine();
            Console.WriteLine($"  → AI chooses: Piece {chosenMove.PieceNumber}");
            
            // Execute move
            var moveResult = game.MovePiece(snapshot.CurrentPlayer, chosenMove.PieceNumber, rollResult.Total);
            
            if (moveResult.Success)
            {
                Console.WriteLine($"  ✓ Moved to {moveResult.NewLocation} position {moveResult.NewPosition}");
                
                if (moveResult.CapturedEnemy)
                {
                    Console.WriteLine($"  ⚔️ CAPTURED an enemy piece!");
                }
                
                if (moveResult.EnteredFinishLane)
                {
                    Console.WriteLine($"  🏁 Entered finish lane!");
                }
                
                if (moveResult.Finished)
                {
                    Console.WriteLine($"  🎉 PIECE FINISHED!");
                    
                    // Check for winner
                    var afterMove = game.GetSnapshot();
                    var finishedCount = afterMove.Pieces
                        .Count(p => p.Color == snapshot.CurrentPlayer && p.Location == PieceLocation.Finished);
                    
                    Console.WriteLine($"  ({finishedCount}/4 pieces finished)");
                    
                    if (finishedCount >= 4)
                    {
                        winner = snapshot.CurrentPlayer;
                    }
                }
            }
            else
            {
                Console.WriteLine($"  ✗ Move failed: {moveResult.Error}");
            }
            
            // Handle extra roll for doubles
            if (rollResult.IsDouble && !rollResult.PenaltyThreeSixes && winner == null)
            {
                Console.WriteLine();
                Console.WriteLine($"  (Extra roll for double...)");
                // Continue same player's turn
            }
            else
            {
                game.EndTurn();
            }
            
            // Small delay for readability
            Thread.Sleep(50);
        }
        
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        
        if (winner != null)
        {
            Console.WriteLine($"  🏆 WINNER: {winner}! 🏆");
        }
        else
        {
            Console.WriteLine($"  Game ended after {maxTurns} turns with no winner.");
        }
        
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        
        // Final state
        var finalState = game.GetSnapshot();
        Console.WriteLine();
        Console.WriteLine("Final piece positions:");
        PrintPieces(finalState.Pieces);
    }
    
    static void PrintPieces(PieceInfo[] pieces)
    {
        var grouped = pieces.GroupBy(p => p.Color);
        foreach (var group in grouped)
        {
            Console.WriteLine($"  {group.Key}:");
            foreach (var piece in group.OrderBy(p => p.PieceNumber))
            {
                var pos = piece.Location == PieceLocation.Home ? "Home" :
                          piece.Location == PieceLocation.Finished ? "FINISHED" :
                          $"{piece.Location} pos {piece.Position}";
                Console.WriteLine($"    Piece {piece.PieceNumber}: {pos}");
            }
        }
    }
    
    static ValidMove ChooseBestMove(ValidMove[] moves)
    {
        // Priority:
        // 1. Finish a piece
        // 2. Capture an enemy
        // 3. Leave home (if possible)
        // 4. Move the piece closest to finish
        
        var finishing = moves.FirstOrDefault(m => m.WouldFinish);
        if (finishing != default) return finishing;
        
        var capturing = moves.FirstOrDefault(m => m.WouldCapture);
        if (capturing != default) return capturing;
        
        var leavingHome = moves.FirstOrDefault(m => m.TargetLocation == PieceLocation.Track && 
                                                     moves.Any(mm => mm.PieceNumber == m.PieceNumber));
        
        // Default: random choice
        return moves[Random.Shared.Next(moves.Length)];
    }
}
