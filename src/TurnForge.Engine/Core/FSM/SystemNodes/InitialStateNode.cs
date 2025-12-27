using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Core.Fsm.SystemNodes
{
    /// <summary>
    /// Initial node - allows board initialization.
    /// Transitions to BoardReady/GamePrepared after board is set up.
    /// </summary>
    public class InitialStateNode : LeafNode
    {
        public InitialStateNode()
        {
            AddAllowedCommand<InitializeBoardCommand>();
        }

        public override bool IsCompleted(GameState state)
        {
            // Transition when board is created
            return state.Board != null;
        }
    }
}
