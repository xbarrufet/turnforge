using TicTacToe.Rules;

Console.WriteLine("═══════════════════════════════════════");
Console.WriteLine("         TIC-TAC-TOE SIMULATION        ");
Console.WriteLine("═══════════════════════════════════════");
Console.WriteLine();

var game = new TicTacToeGame();
game.NewGame();

Console.WriteLine("Initial board:");
game.PrintBoard();
Console.WriteLine();

// Simple AI: random moves
var random = new Random();
int moveCount = 0;

while (game.GetSnapshot().Result == GameResult.InProgress)
{
    moveCount++;
    var snapshot = game.GetSnapshot();
    
    // Find empty positions
    var emptyPositions = Enumerable.Range(0, 9)
        .Where(i => snapshot.Board[i] == CellState.Empty)
        .ToList();
    
    // Pick random position
    var position = emptyPositions[random.Next(emptyPositions.Count)];
    
    Console.WriteLine($"Move {moveCount}: {snapshot.CurrentPlayer} places at position {position}");
    
    var result = game.PlaceMark(position);
    
    if (!result.Success)
    {
        Console.WriteLine($"  ERROR: {result.Error}");
        continue;
    }
    
    game.PrintBoard();
    Console.WriteLine();
    
    if (result.Result != GameResult.InProgress)
    {
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine(result.Result switch
        {
            GameResult.XWins => "  🏆 X WINS! 🏆",
            GameResult.OWins => "  🏆 O WINS! 🏆",
            GameResult.Draw => "  🤝 DRAW! 🤝",
            _ => ""
        });
        Console.WriteLine("═══════════════════════════════════════");
    }
}

Console.WriteLine();
Console.WriteLine($"Game finished in {moveCount} moves.");
