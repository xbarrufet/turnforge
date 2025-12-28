using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Builders;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Core.Fsm.Builders;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Core.Orchestrator;

namespace TicTacToe.Rules;

// ============================================================================
// Enums
// ============================================================================

public enum Player { X, O }
public enum CellState { Empty, X, O }
public enum GameResult { InProgress, XWins, OWins, Draw }

// ============================================================================
// Board Logic
// ============================================================================

public static class TicTacToeBoard
{
    public const int Size = 3;
    public const int TotalCells = 9;
    
    public static readonly int[][] WinningLines = new[]
    {
        new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 }, // Rows
        new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, // Cols
        new[] { 0, 4, 8 }, new[] { 2, 4, 6 }  // Diagonals
    };
    
    public static CellState[] CreateEmptyBoard() => new CellState[TotalCells];
    
    public static GameResult CheckResult(CellState[] board)
    {
        foreach (var line in WinningLines)
        {
            var a = board[line[0]];
            if (a != CellState.Empty && a == board[line[1]] && a == board[line[2]])
                return a == CellState.X ? GameResult.XWins : GameResult.OWins;
        }
        return board.All(c => c != CellState.Empty) ? GameResult.Draw : GameResult.InProgress;
    }
}

// ============================================================================
// Bootstrap
// ============================================================================

public static class TicTacToeBootstrap
{
    public static GameState CreateInitialState()
    {
        return GameState.Empty()
            .WithMetadata("Board", TicTacToeBoard.CreateEmptyBoard())
            .WithMetadata("CurrentPlayer", Player.X)
            .WithMetadata("GameResult", GameResult.InProgress);
    }
}

// ============================================================================
// Commands
// ============================================================================

public class PlaceMarkCommand : TurnForge.Engine.Commands.Interfaces.ICommand
{
    public int Position { get; init; }
    public Type CommandType => typeof(PlaceMarkCommand);
}

// ============================================================================
// Workflow Context (concrete implementation)
// ============================================================================

public class TicTacToeContext : WorkflowContext
{
    public TicTacToeContext(GameState state)
    {
        InitializeState(state);
    }
}

// ============================================================================
// Typed Workflow Data
// ============================================================================

public record PlaceMarkData(int Position) : IWorkflowData;

// ============================================================================
// Workflow Factory
// ============================================================================

public static class PlaceMarkWorkflowFactory
{
    public static IWorkflow Create()
    {
        return WorkflowBuilder.Create("TicTacToe.PlaceMark")
            .AddNode(new ValidatePlacementNode())
            .AddNode(new PlaceMarkNode())
            .AddNode(new CheckResultNode())
            .AddNode(new SwitchPlayerNode())
            .Build();
    }
}

// ============================================================================
// Workflow Nodes
// ============================================================================

public class ValidatePlacementNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        var data = context.GetTypedData<PlaceMarkData>();
        
        if (data.Position < 0 || data.Position >= TicTacToeBoard.TotalCells)
            return ValidationResult.CancelResult;
        
        var board = (CellState[])context.State.Metadata["Board"];
        var result = (GameResult)context.State.Metadata["GameResult"];
        
        if (result != GameResult.InProgress || board[data.Position] != CellState.Empty)
            return ValidationResult.CancelResult;
        
        return ValidationResult.OkResult;
    }
}

public class PlaceMarkNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        var data = context.GetTypedData<PlaceMarkData>();
        var currentPlayer = (Player)context.State.Metadata["CurrentPlayer"];
        
        var board = ((CellState[])context.State.Metadata["Board"]).ToArray();
        board[data.Position] = currentPlayer == Player.X ? CellState.X : CellState.O;
        
        var newState = context.State.WithMetadata("Board", board);
        context.RecordDecision(new UpdateStateDecision(newState));
        
        return ValidationResult.OkResult;
    }
}

public class CheckResultNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        var board = (CellState[])context.State.Metadata["Board"];
        var result = TicTacToeBoard.CheckResult(board);
        
        context.RecordDecision(new UpdateStateDecision(
            context.State.WithMetadata("GameResult", result)));
        
        return ValidationResult.OkResult;
    }
}

public class SwitchPlayerNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        var result = (GameResult)context.State.Metadata["GameResult"];
        
        if (result == GameResult.InProgress)
        {
            var current = (Player)context.State.Metadata["CurrentPlayer"];
            var next = current == Player.X ? Player.O : Player.X;
            context.RecordDecision(new UpdateStateDecision(
                context.State.WithMetadata("CurrentPlayer", next)));
        }
        
        return ValidationResult.OkResult;
    }
}

// ============================================================================
// Decision
// ============================================================================

public class UpdateStateDecision : IDecision
{
    private readonly GameState _newState;
    public UpdateStateDecision(GameState newState) => _newState = newState;
    public DecisionTiming Timing => DecisionTiming.Immediate;
    public string OriginId => "TicTacToe.UpdateState";
    public GameState Apply(GameState state) => _newState;
}

// ============================================================================
// Game Engine (uses WorkflowOrchestrator)
// ============================================================================

public class TicTacToeGame
{
    private GameState _state;
    private readonly WorkflowOrchestrator _orchestrator;
    private readonly IWorkflow _placeMarkWorkflow;
    
    public TicTacToeGame()
    {
        _state = GameState.Empty();
        _orchestrator = new WorkflowOrchestrator();
        _placeMarkWorkflow = PlaceMarkWorkflowFactory.Create();
    }
    
    public void NewGame()
    {
        _state = TicTacToeBootstrap.CreateInitialState();
    }
    
    /// <summary>
    /// Place a mark using the Command → Workflow → State flow.
    /// </summary>
    public PlaceMarkResult PlaceMark(int position)
    {
        // Validation
        var result = (GameResult)_state.Metadata["GameResult"];
        if (result != GameResult.InProgress)
            return new PlaceMarkResult(false, "Game is over");
        
        if (position < 0 || position >= TicTacToeBoard.TotalCells)
            return new PlaceMarkResult(false, "Invalid position");
        
        var board = (CellState[])_state.Metadata["Board"];
        if (board[position] != CellState.Empty)
            return new PlaceMarkResult(false, "Cell already occupied");
        
        var currentPlayer = (Player)_state.Metadata["CurrentPlayer"];
        
        // Create context with current state and typed data
        var context = new TicTacToeContext(_state);
        context.SetTypedData(new PlaceMarkData(position));
        
        // Execute workflow through orchestrator
        var executionResult = _orchestrator.Execute(_placeMarkWorkflow, context);
        
        // Decisions are already applied to context.State by RecordDecision
        _state = context.State;
        
        var finalResult = (GameResult)_state.Metadata["GameResult"];
        
        return new PlaceMarkResult(true, null, currentPlayer, position, finalResult);
    }
    
    public GameSnapshot GetSnapshot() => new(
        ((CellState[])_state.Metadata["Board"]).ToArray(),
        (Player)_state.Metadata["CurrentPlayer"],
        (GameResult)_state.Metadata["GameResult"]);
    
    public GameState GetState() => _state;
    public void SetState(GameState state) => _state = state;
    
    public void PrintBoard()
    {
        var board = (CellState[])_state.Metadata["Board"];
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var cell = board[row * 3 + col];
                Console.Write($" {(cell == CellState.Empty ? "." : cell.ToString())} ");
                if (col < 2) Console.Write("|");
            }
            Console.WriteLine();
            if (row < 2) Console.WriteLine("---+---+---");
        }
    }
}

// ============================================================================
// DTOs
// ============================================================================

public record PlaceMarkResult(
    bool Success,
    string? Error = null,
    Player? Player = null,
    int? Position = null,
    GameResult Result = GameResult.InProgress);

public record GameSnapshot(
    CellState[] Board,
    Player CurrentPlayer,
    GameResult Result);
