using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions; // For MissionDefinition
using TurnForge.Engine.Entities.Overlay; // For IGameStateOperation
using TurnForge.Engine.ValueObjects; // For EntityId

namespace TurnForge.Engine.Commands.StartGame.Action.Operations;

public sealed class CreateBoardOperation : IGameStateOperation
{
    public EntityId Target => EntityId.Empty;
    
    private readonly IGameBoard _board;
    private readonly MissionDefinition? _mission;
    
    public CreateBoardOperation(IGameBoard board, MissionDefinition? mission = null)
    {
        _board = board;
        _mission = mission;
    }
    
    public void Apply(IGameStateMutator mutator)
    {
        mutator.SetBoard(_board);
        if (_mission != null)
        {
            mutator.SetMission(_mission);
        }
    }
}
