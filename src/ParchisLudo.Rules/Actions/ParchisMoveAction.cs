using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Overlay; // MoveOperation is here
using TurnForge.Engine.Definitions; // For GameEntity
using TurnForge.Engine.Definitions.Actors; // For Actor
using Parchis.Rules.Logic;
using Parchis.Rules.Board;
using Parchis.Rules.Extensions; // For ParchisViewExtensions

namespace Parchis.Rules.Actions;

public static class ParchisMoveActionFactory
{
    public const string ActionId = "parchis_move";
    
    public static IAction Create()
    {
        var ruleOfFive = new RuleOfFiveNode();
        var selectPawn = new SelectPawnNode();
        var executeMove = new ExecuteMoveNode();
        
        ruleOfFive.SetNextNode(selectPawn);
        selectPawn.SetNextNode(executeMove);
        
        return ActionBuilder.Create(ActionId)
            .AddNode(ruleOfFive)
            .AddNode(selectPawn)
            .AddNode(executeMove)
            .Build();
    }
}

public class RuleOfFiveNode : LinkableNode
{
    public override NodeId Id => new("RuleOfFive_Check");
    
    public override ActionStepResult Execute(ActionContext context)
    {
        if (!context.TryGet<int>("Roll", out var roll))
            return ActionStepResult.Fail("Roll not set");
            
        if (!context.TryGet<PlayerId>("PlayerId", out var playerId))
            return ActionStepResult.Fail("PlayerId not set");

        if (roll != 5)
        {
            return ActionStepResult.Success();
        }
        
        var state = context.State;
        var view = new GameStateView(state, context.Overlay);
        
        // Use extension method for semantic API
        var pawns = view.GetPawns(playerId).ToList();
        if (pawns.Count == 0) return ActionStepResult.Success();
        
        var color = pawns[0].Team?.ToLower();
        if (string.IsNullOrEmpty(color)) return ActionStepResult.Success();
        
        // Use extension method: GetPawnsInSpawn
        var pawnsInSpawn = view.GetPawnsInSpawn(playerId, color).ToList();
        
        if (pawnsInSpawn.Count == 0)
        {
            return ActionStepResult.Success();
        }
        
        var pawnToSpawn = pawnsInSpawn[0];
        
        var entryTileId = GetEntryTileForColor(color!);
        var targetPos = new TilePosition(new TileId(entryTileId));
        
        context.Overlay.Record(new MoveOperation(pawnToSpawn.Id, targetPos));
        
        context.Set("MoveHandled", true);
        
        return ActionStepResult.Success();
    }
    
    private string GetEntryTileForColor(string color)
    {
        return color switch
        {
            "red" => ParchisBoard.RedEntry,
            "blue" => ParchisBoard.BlueEntry,
            "green" => ParchisBoard.GreenEntry,
            "yellow" => ParchisBoard.YellowEntry,
            _ => "track_1"
        };
    }
}

public class SelectPawnNode : LinkableNode
{
    public override NodeId Id => new("Select_Pawn");
    
    public override ActionStepResult Execute(ActionContext context)
    {
        if (context.TryGet<bool>("MoveHandled", out var handled) && handled)
        {
            return ActionStepResult.Success();
        }
        
        if (!context.TryGet<int>("Roll", out var roll)) return ActionStepResult.Fail("Roll missing");
        if (!context.TryGet<PlayerId>("PlayerId", out var playerId)) return ActionStepResult.Fail("PlayerId missing");
        
        var view = new GameStateView(context.State, context.Overlay);
        var pawns = view.GetEntitiesForOwner(playerId).ToList();
        var color = pawns.FirstOrDefault()?.GetComponent<TurnForge.Engine.Components.Interfaces.ITeamComponent>()?.Team?.ToLower();
        
        if (color == null) return ActionStepResult.Fail("Player has no color/team");
        
        foreach (var pawn in pawns)
        {
            var pos = view.GetPosition(pawn.Id);
            if (pos is TilePosition tp)
            {
                var dest = ParchisMoveLogic.CalculateDestination(view, tp.TileId, roll, color, out bool center, out bool bounce);
                if (dest != null)
                {
                    context.Set("SelectedPawnId", pawn.Id);
                    context.Set("TargetDestination", dest.Value);
                    context.Set("MoveBounced", bounce);
                    context.Set("ReachedCenter", center);
                    return ActionStepResult.Success();
                }
            }
        }
        
        context.Set("NoMovesPossible", true);
        return ActionStepResult.Success();
    }
}

public class ExecuteMoveNode : LinkableNode
{
    public override NodeId Id => new("Execute_Move");

    public override ActionStepResult Execute(ActionContext context)
    {
        if (context.TryGet<bool>("MoveHandled", out var handled) && handled) return ActionStepResult.Success();
        if (context.TryGet<bool>("NoMovesPossible", out var noMoves) && noMoves) return ActionStepResult.Success();
        
        if (!context.TryGet<EntityId>("SelectedPawnId", out var pawnId)) return ActionStepResult.Success();
        if (!context.TryGet<TileId>("TargetDestination", out var dest)) return ActionStepResult.Fail("No destination");
        
        context.Overlay.Record(new MoveOperation(pawnId, new TilePosition(dest)));
        
        return ActionStepResult.Success();
    }
}

