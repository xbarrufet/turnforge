using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core;

public interface IGameLoop
{
    GameLoopResult Validate(ICommand command);
}