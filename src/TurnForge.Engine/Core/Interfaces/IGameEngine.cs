using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core.Interfaces;

public interface IGameEngine
{
    CommandResult Send(ICommand command);
    void Subscribe(Action<IGameEffect> handler);
}
