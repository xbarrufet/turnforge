using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Orchestrator;

namespace TurnForge.Engine.Core.Interfaces;

public interface IGameEngine
{
    CommandTransaction ExecuteCommand(ICommand command);

}
