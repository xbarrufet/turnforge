using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Core.Fsm.SystemNodes
{
    public class GamePreparedNode : LeafNode
    {
        public GamePreparedNode()
        {
            AddAllowedCommand<SpawnAgentsCommand>();
        }

        public override bool IsCompleted(GameState state)
        {
            // Proceed when agents are spawned
            return state.Agents.Count > 0;
        }
    }
}
