using TurnForge.Engine.Core.Action.Interfaces;
using Parchis.Rules.Actions;

namespace Parchis.Rules.Factory; 

public static class ParchisActionRegistration
{
    public static void Register(IActionRegistry registry)
    {
        // 1. Gameplay Actions
        registry.Register(ParchisActions.Move, ParchisMoveActionFactory.Create);
        
        // 2. Initialization Actions
        registry.Register(ParchisActions.StartGame, ParchisStartGameActionFactory.Create);
    }
}
