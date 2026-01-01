using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Entities;

namespace TurnForge.Engine.Commands.StartGame.Workflow.Operations;

/// <summary>
/// Operation to create the board and set the mission data.
/// </summary>
public class CreateBoardOperation : IGameStateOperation
{
    public EntityId Target => EntityId.Empty; // Not targeting a specific entity
    
    private readonly IGameBoard _board;
    private readonly MissionData? _mission;
    
    public CreateBoardOperation(IGameBoard board, MissionData? mission = null)
    {
        _board = board;
        _mission = mission;
    }
    
    public void Apply(IGameStateMutator mutator)
    {
        mutator.SetBoard(_board);
        // mutator.SetMission(_mission); // Mutator doesn't support null directly based on lint? 
        // Logic: if _mission != null, call SetMission.
        // Lint said != cannot be applied to MissionData? and null. Wait, record IS nullable?
        // Ah, if MissionData is struct? No, it's record (class).
        // Let's re-check the lint: "Operator '!=' cannot be applied to operands of type 'MissionData?' and '<null>'"
        // This implies MissionData might be a struct or unconstrained generic?
        // No, it's defined as `public record MissionData`. Records are reference types unless `record struct`.
        // Maybe I created it as `record struct`?
        // I created it (step 1076): `public record MissionData`. That is reference type.
        // Maybe I need using System?
        
        if (_mission is not null)
        {
            mutator.SetMission(_mission);
        }
    }
}
