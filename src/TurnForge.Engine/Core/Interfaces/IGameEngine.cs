using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Orchestrator;

namespace TurnForge.Engine.Core.Interfaces;

public interface IGameEngine
{
    CommandTransaction ExecuteCommand(ICommand command);
    void SetFsmController(global::TurnForge.Engine.Core.Fsm.FsmController controller);
    void InitializeBoard(global::TurnForge.Engine.Entities.Board.Descriptors.BoardDescriptor descriptor);

}
