
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Interfaces;

namespace TurnForge.Engine.Core.Fsm.SystemNodes
{
    public class GamePreparedNode : LeafNode
    {
        public GamePreparedNode()
        {
            AddAllowedCommand<SpawnAgentsCommand>();
        }

        public override bool IsCommandValid(ICommand command, GameState state)
        {
            return command is SpawnAgentsCommand;
        }

        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = result.Tags != null && result.Tags.Contains("AgentsSpawned");
            return Enumerable.Empty<IFsmApplier>();
        }
    }
}
