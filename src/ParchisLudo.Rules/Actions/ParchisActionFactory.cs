using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Actions;

namespace Parchis.Rules.Factory; 

public static class ParchisActionRegistration
{
    public static void Register(IActionRegistry registry)
    {
        // Gameplay Actions
        registry.Register(ParchisActions.Move, ParchisMoveAction.Create);
        
    }
}
