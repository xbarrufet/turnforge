using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Interfaces;

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

    public override bool IsCommandValid(ICommand command, GameState state)
    {
        return command is SpawnPropsCommand;
    }

    public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
    {
        transitionRequested = result.Tags != null && result.Tags.Contains("PropsSpawned");
        return Enumerable.Empty<IFsmApplier>();
    }
}
