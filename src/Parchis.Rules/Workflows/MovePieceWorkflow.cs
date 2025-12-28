using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Builders;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Board;
using Parchis.Rules.Api;

namespace Parchis.Rules.Workflows;

// ============================================================================
// Typed Workflow Data
// ============================================================================

/// <summary>
/// Typed data for piece movement context.
/// </summary>
public record MovePieceData(
    int PieceNumber,
    int Steps,
    PieceInfo? ValidatedPiece = null,
    PieceInfo? MovedPiece = null
) : IWorkflowData;

// ============================================================================
// Workflow Factory (using builder)
// ============================================================================

/// <summary>
/// Factory to create the MovePiece workflow.
/// </summary>
public static class MovePieceWorkflowFactory
{
    public static IWorkflow Create()
    {
        return WorkflowBuilder.Create("Parchis.MovePiece")
            .AddNode(new ValidateMoveNode())
            .AddNode(new ExecuteMoveNode())
            .AddNode(new CheckCaptureNode())
            .WithReaction<CaptureReaction>()
            .WithReaction<SafeZoneReaction>()
            .Build();
    }
}

// ============================================================================
// Nodes (using LinkableNode for builder linking)
// ============================================================================

public class ValidateMoveNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        // Get typed data
        var data = context.GetTypedData<MovePieceData>();
        var currentPlayer = (PlayerColor)context.State.Metadata["CurrentPlayer"];
        var pieceId = $"{currentPlayer}_{data.PieceNumber}";
        
        // Check if piece exists
        if (!context.TryGet<Dictionary<string, PieceInfo>>("Pieces", out var pieces) || 
            !pieces.TryGetValue(pieceId, out var piece))
        {
            return ValidationResult.CancelResult;
        }
        
        // Validate move
        if (piece.Location == PieceLocation.Home && data.Steps != 5)
            return ValidationResult.CancelResult;
        
        if (piece.Location == PieceLocation.Finished)
            return ValidationResult.CancelResult;
        
        // Store validated piece
        context.SetTypedData(data with { ValidatedPiece = piece });
        return ValidationResult.OkResult;
    }
}

public class ExecuteMoveNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        var data = context.GetTypedData<MovePieceData>();
        var piece = data.ValidatedPiece!;
        var currentPlayer = (PlayerColor)context.State.Metadata["CurrentPlayer"];
        
        PieceInfo newPiece;
        
        if (piece.Location == PieceLocation.Home)
        {
            var entryPoint = currentPlayer == PlayerColor.Yellow 
                ? ParchisBoard.YellowEntry 
                : ParchisBoard.BlueEntry;
            newPiece = piece with { Location = PieceLocation.Track, Position = entryPoint };
        }
        else if (piece.Location == PieceLocation.Track)
        {
            var newPosition = ParchisBoard.GetNextPosition(piece.Position, data.Steps);
            newPiece = piece with { Position = newPosition };
        }
        else // FinishLane
        {
            var newPosition = piece.Position + data.Steps;
            newPiece = newPosition >= ParchisBoard.FinishLaneSize
                ? piece with { Location = PieceLocation.Finished, Position = 0 }
                : piece with { Position = newPosition };
        }
        
        // Store moved piece
        context.SetTypedData(data with { MovedPiece = newPiece });
        
        // Emit move event
        context.AddEvent(new PieceMovedEvent(
            currentPlayer,
            piece.PieceNumber,
            piece.Location,
            piece.Position,
            newPiece.Location,
            newPiece.Position));
        
        return ValidationResult.OkResult;
    }
}

public class CheckCaptureNode : LinkableNode
{
    public override NodeId Id { get; } = NodeId.New();
    
    public override ValidationResult Validate(WorkflowContext context)
    {
        var newState = context.State.WithMetadata("MoveCompleted", true);
        context.RecordDecision(new UpdateStateDecision(newState));
        return ValidationResult.OkResult;
    }
}

// ============================================================================
// Events
// ============================================================================

public record PieceMovedEvent(
    PlayerColor Player,
    int PieceNumber,
    PieceLocation FromLocation,
    int FromPosition,
    PieceLocation ToLocation,
    int ToPosition) : IWorkflowEvent;

public record PieceCapturedEvent(
    PlayerColor CapturedBy,
    PlayerColor CapturedPlayer,
    int CapturedPieceNumber,
    int Position) : IWorkflowEvent;

// ============================================================================
// Reactions
// ============================================================================

public class CaptureReaction : IReaction
{
    public ReactionId Id => new("Parchis.Capture");
    
    public bool CanReact(WorkflowContext context)
    {
        return context.PendingEvents.OfType<PieceMovedEvent>()
            .Any(e => e.ToLocation == PieceLocation.Track && !ParchisBoard.IsSafeZone(e.ToPosition));
    }
    
    public ReactionResult React(WorkflowContext context, IInputActionResult? input)
    {
        if (!context.TryGet<Dictionary<string, PieceInfo>>("Pieces", out var pieces))
            return ReactionResult.NoChange(context);
        
        foreach (var evt in context.PendingEvents.OfType<PieceMovedEvent>())
        {
            if (evt.ToLocation != PieceLocation.Track || ParchisBoard.IsSafeZone(evt.ToPosition))
                continue;
            
            var enemies = pieces.Values
                .Where(p => p.Color != evt.Player && p.Location == PieceLocation.Track && p.Position == evt.ToPosition)
                .ToList();
            
            foreach (var enemy in enemies)
            {
                context.AddEvent(new PieceCapturedEvent(evt.Player, enemy.Color, enemy.PieceNumber, evt.ToPosition));
            }
        }
        
        return ReactionResult.NoChange(context);
    }
}

public class SafeZoneReaction : IReaction
{
    public ReactionId Id => new("Parchis.SafeZone");
    public bool CanReact(WorkflowContext context) => false;
    public ReactionResult React(WorkflowContext context, IInputActionResult? input) => ReactionResult.NoChange(context);
}
