using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Core;

public sealed class GameLoop : IGameLoop
{
    public GameLoopResult Validate(ICommand command)
    {
        // Fase 2: todo permitido
        return GameLoopResult.Allowed(
            requiresAck: false,
            domainResult: null
        );
    }
}