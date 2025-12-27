using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Core.Fsm.SystemNodes;

/// <summary>
/// Node after board is initialized, ready to spawn props.
/// Transitions to GamePrepared after props are spawned.
/// </summary>
public class BoardReadyNode : LeafNode
{
    public BoardReadyNode()
    {
        AddAllowedCommand<SpawnPropsCommand>();
    }

    public override bool IsCompleted(GameState state)
    {
        // Require at least one prop? Or just check if command execution happened?
        // Since we don't track execution history easily without orchestration, checking state is best.
        // Assuming at least one prop exists (e.g. Spawn Points).
        return state.GetProps().Any();
    }
}
