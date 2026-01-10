using TurnForge.Engine.Commands;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Nodes;

public class StartGameNode(BaseFsmNode? rootNode) : BaseFsmNode("StartGameNode")
{
    public override bool IsCompleted(GameStateView state)
    {
        return state.IsGameStarted;
    }
    
    public override BaseFsmNode? GetNextNode(GameStateView state)
    {
        return rootNode;
    }

    public override IReadOnlyList<ActionId> GetAllowedActions()
    {
        return [CoreActions.StartGameActionId];   
    }
}