namespace ParchisLudo.Rules.Actions;

public static class ParchisMoveAction
{
    /*
    public const string ActionIdString = "parchis_move";

    public static ActionId ActionId() => new(ActionIdString);

    public static IAction Create()
    {
        var selectPawn = new SelectPawnNode();
        var executeMove = new ExecuteMoveNode();

        selectPawn.SetNextNode(executeMove);

        return ActionBuilder.Create(ActionIdString)
            .WithContext(() => new MoveActionContext())
            .AddNode(selectPawn)
            .AddNode(executeMove)
            .Build();
    }
}

/// <summary>
/// Helper to get current player - from parameter or TurnOrder.x
/// </summary>



public class SelectPawnNode : LinkableNode
{
    public override NodeId Id => new("Select_Pawn");

    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        Console.WriteLine("SelectPawnNode: Starting pawn selection");
        var ctx = GetTypedContext<MoveActionContext>(context);
        if (!ctx.TryGet<int>("Roll", out var roll))
        {
            Console.WriteLine("SelectPawnNode: ERROR - Roll missing");
            return ActionStepResult.Fail("Roll missing");
        }

        var playerId = state.TurnOrder.CurrentPlayer;
        Console.WriteLine($"SelectPawnNode: Player={playerId}, Roll={roll}");

        // Check if roll is 5 and there are pawns in spawn
        var pawnsInSpawn = state.Query()
                                .ControlledBy(playerId)
                                .InSpawn()
                                .OfType<Agent>()
                                .Execute().ToList();
        var pawnsOnTrack = state.Query()
                                .ControlledBy(playerId)
                                .NotInSpawn()
                                .OfType<Agent>()
                                .Execute().ToList();
        Console.WriteLine($"SelectPawnNode: player={playerId} Pawns in spawn={pawnsInSpawn.Count} in track={pawnsOnTrack.Count}");

        if (roll == 5 && pawnsInSpawn.Count > 0)
        {
            Console.WriteLine($"SelectPawnNode: Moving pawn from spawn: {pawnsInSpawn[0].Name}");
            ctx.MovePawnFromSpawn = true;
            ctx.SelectedPawn = pawnsInSpawn[0];
            return ActionStepResult.Success();
        }


        Console.WriteLine($"SelectPawnNode: Pawns on track={pawnsOnTrack.Count}");

        if (pawnsOnTrack.Count == 0)
        {
            Console.WriteLine("SelectPawnNode: ERROR - No pawns available to move");
            return ActionStepResult.Fail("No pawns available");
        }

        ctx.MovePawnFromSpawn = false;
        ctx.SelectedPawn = pawnsOnTrack[0];
        Console.WriteLine($"SelectPawnNode: Selected pawn on track: {ctx.SelectedPawn.Name} at {state.GetPosition(ctx.SelectedPawn.Id)}");
        return ActionStepResult.Success();
    }

}


public class ExecuteMoveNode : LinkableNode
{
    public override NodeId Id => new("Execute_Move");

    public override ActionStepResult Execute(ActionContext context, GameStateView state)
    {
        Console.WriteLine("ExecuteMoveNode: Starting move execution");
        var ctx = GetTypedContext<MoveActionContext>(context);
        var playerId = state.TurnOrder.CurrentPlayer;

        if (ctx.MovePawnFromSpawn)
        {
            Console.WriteLine($"ExecuteMoveNode: Moving pawn {ctx.SelectedPawn.Name} from spawn");
            var moveOp = CreateMoveFromSpawnOperation(ctx.SelectedPawn);
            Console.WriteLine($"ExecuteMoveNode: Target position={moveOp.NewPosition}");
            state.RecordOperation(moveOp);
            state.RecordOperation(new SpendAPOperation(playerId, EntityId.Empty, 1));
            Console.WriteLine("ExecuteMoveNode: Move from spawn completed");
            return ActionStepResult.Success();
        }

        var currentPos = state.GetPosition(ctx.SelectedPawn.Id);
        Console.WriteLine($"ExecuteMoveNode: Moving pawn {ctx.SelectedPawn.Name} from {currentPos}");

        var newPosition = MoveForward((Actor)ctx.SelectedPawn, state);
        Console.WriteLine($"ExecuteMoveNode: Target position={newPosition}");

        state.RecordOperation(new MoveOperation(ctx.SelectedPawn.Id, newPosition));
        state.RecordOperation(new SpendAPOperation(playerId, EntityId.Empty, 1));
        Console.WriteLine("ExecuteMoveNode: Move completed");
        return ActionStepResult.Success();
    }

    private MoveOperation CreateMoveFromSpawnOperation(GameEntity pawn)
    {
        Console.WriteLine($"CreateMoveFromSpawn: Creating spawn move for {pawn.Name}");
        if (!pawn.TryGetTrait<ColorTrait>(out var colorTrait) || colorTrait == null)
        {
            Console.WriteLine("CreateMoveFromSpawn: ERROR - Pawn missing ColorTrait");
            throw new InvalidOperationException("Pawn must have ColorTrait");
        }

        var newPosition = ParchisBoard.GetEntryForColor(colorTrait.Color);
        Console.WriteLine($"CreateMoveFromSpawn: Entry position for {colorTrait.Color}={newPosition}");
        return new MoveOperation(pawn.Id, newPosition);
    }

    private TilePosition MoveForward(Actor pawn, GameStateView state)
    {
        Console.WriteLine($"MoveForward: Calculating forward position for {pawn.Name}");
        var currentPos = state.GetPosition(pawn.Id);
        Console.WriteLine($"MoveForward: Current position={currentPos}");

        var newPos = state.GetForwardPosition(pawn);
        Console.WriteLine($"MoveForward: New position={newPos}");
        return newPos;
    }

*/

}

