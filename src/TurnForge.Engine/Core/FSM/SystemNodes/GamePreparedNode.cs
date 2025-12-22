
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Interfaces;

namespace TurnForge.Engine.Core.Fsm.SystemNodes
{
    public class GamePreparedNode : LeafNode
    {
        public GamePreparedNode()
        {
            //AddAllowedCommand<SpawnAgentsCommand>();
            AddAllowedCommand<StartGameCommand>();
        }

        public override bool IsCommandValid(ICommand command, GameState state)
        {
            return command is SpawnAgentsCommand || command is StartGameCommand;
        }

        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = result.Tags != null && (result.Tags.Contains("AgentsSpawned") || result.Tags.Contains("GameStarted"));
            return Enumerable.Empty<IFsmApplier>();
        }
    }
}
